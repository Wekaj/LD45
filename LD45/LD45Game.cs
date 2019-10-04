﻿using LD45.Graphics;
using LD45.Input;
using LD45.Screens;
using LD45.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LD45 {
    public sealed class LD45Game : Game {
        private readonly GraphicsDeviceManager _graphics;
        private readonly ServiceContainer _services;
        private readonly ScreenManager _screens;
        private readonly InputManager _input;

        public LD45Game() {
            _graphics = new GraphicsDeviceManager(this);
            _services = new ServiceContainer();
            _screens = new ScreenManager(_services);
            _input = new InputManager();

            IsMouseVisible = true;

            Content.RootDirectory = "Content";

            Window.AllowUserResizing = true;
        }

        protected override void Initialize() {
            base.Initialize();

            InitializeServices();

            _screens.Push(new TitleScreen());
        }

        protected override void LoadContent() {
        }

        protected override void UnloadContent() {
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Exit();
            }

            _input.Update();

            _screens.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.TransparentBlack);

            _screens.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void InitializeServices() {
            _services.SetService(Content);

            AddBindings(_input.Bindings);
            _services.SetService(_input);

            _services.SetService(new Renderer2D(GraphicsDevice, Window, 2));
        }

        private void AddBindings(InputBindings bindings) {
            bindings.Set(BindingId.LeftClick, new MouseBinding(MouseButtons.Left));
            bindings.Set(BindingId.MiddleClick, new MouseBinding(MouseButtons.Middle));
            bindings.Set(BindingId.RightClick, new MouseBinding(MouseButtons.Right));
        }
    }
}
