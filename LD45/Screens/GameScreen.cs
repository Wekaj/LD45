﻿using Artemis;
using Artemis.Manager;
using LD45.Components;
using LD45.Controllers;
using LD45.Graphics;
using LD45.Systems;
using LD45.Tiles;
using LD45.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD45.Screens {
    public sealed class GameScreen : IScreen {
        private readonly EntityWorld _entityWorld = new EntityWorld();
        private readonly TileMap _tileMap = new TileMap(64, 64);
        private readonly Camera2D _camera = new Camera2D();

        private ServiceContainer _screenServices;

        private Renderer2D _renderer;
        private RendererSettings _rendererSettings;
        private TileMapRenderer _tileMapRenderer;
        private SquadController _squadController;

        private Texture2D _personTexture;

        public event ScreenEventHandler PushedScreen;
        public event ScreenEventHandler ReplacedSelf;
        public event EventHandler PoppedSelf;

        public void Initialize(IServiceProvider services) {
            CreateServiceContainer(services);

            _renderer = _screenServices.GetRequiredService<Renderer2D>();
            _rendererSettings = new RendererSettings {
                SamplerState = SamplerState.PointClamp
            };
            _tileMapRenderer = new TileMapRenderer(_screenServices);
            _squadController = new SquadController(_screenServices);

            LoadContent(_screenServices);
            InitializeSystems(_screenServices);

            for (int y = 0; y < _tileMap.Height; y++) {
                for (int x = 0; x < _tileMap.Width; x++) {
                    _tileMap[x, y] = new Tile();
                }
            }

            Entity person = _entityWorld.CreateEntity();
            person.AddComponent(new BodyComponent());
            person.AddComponent(new TransformComponent());
            person.AddComponent(new SpriteComponent {
                Texture = _personTexture
            });
            person.AddComponent(new CommanderComponent());
        }

        private void CreateServiceContainer(IServiceProvider services) {
            _screenServices = new ServiceContainer(services);

            _screenServices.SetService(_entityWorld);
            _screenServices.SetService(_tileMap);
            _screenServices.SetService(_camera);
        }

        private void LoadContent(IServiceProvider services) {
            var content = services.GetRequiredService<ContentManager>();

            _personTexture = content.Load<Texture2D>("Textures/Person");
        }

        private void InitializeSystems(IServiceProvider services) {
            _entityWorld.SystemManager.SetSystem(new CommanderMovementSystem(), GameLoopType.Update);
            _entityWorld.SystemManager.SetSystem(new BodyPhysicsSystem(), GameLoopType.Update);
            _entityWorld.SystemManager.SetSystem(new BodyTransformSystem(), GameLoopType.Update);

            _entityWorld.SystemManager.SetSystem(new PathDrawingSystem(services), GameLoopType.Draw);
            _entityWorld.SystemManager.SetSystem(new SpriteDrawingSystem(services), GameLoopType.Draw);
        }

        public void Update(GameTime gameTime) {
            _squadController.Update();

            _entityWorld.Update(gameTime.ElapsedGameTime.Ticks);
        }

        public void Draw(GameTime gameTime) {
            _rendererSettings.TransformMatrix = _camera.GetTransformMatrix();
            _renderer.Begin(_rendererSettings);

            _tileMapRenderer.Draw(_tileMap);
            _entityWorld.Draw();

            _renderer.End();
        }
    }
}
