﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LudumDareTemplate.Graphics {
    public sealed class Renderer {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        private readonly GameWindow _window;
        private RenderTarget2D _renderTarget;
        private bool _boundsUpdatePending = false;

        private RenderTargetBinding[] _defaultTargets;

        public Renderer(GraphicsDevice graphicsDevice, GameWindow window) {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);

            _window = window;
            _renderTarget = new RenderTarget2D(graphicsDevice, window.ClientBounds.Width, window.ClientBounds.Height);

            Bounds = window.ClientBounds;

            window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        public Rectangle Bounds { get; private set; }

        public void Begin() {
            _defaultTargets = _graphicsDevice.GetRenderTargets();
            _graphicsDevice.SetRenderTarget(_renderTarget);
            _graphicsDevice.Clear(Color.TransparentBlack);

            _spriteBatch.Begin();
        }

        public void Draw(Texture2D texture, Vector2 position,
            Rectangle? sourceRectangle = null, Vector2? origin = null, float rotation = 0f) {

            _spriteBatch.Draw(texture, position, sourceRectangle, Color.White, rotation, origin ?? Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
        }

        public void Draw(SpriteFont font, string text, Vector2 position,
            Color? color = null) {

            _spriteBatch.DrawString(font, text, position, color ?? Color.White);
        }

        public void End() {
            _spriteBatch.End();

            _graphicsDevice.SetRenderTargets(_defaultTargets);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();

            if (_boundsUpdatePending) {
                Bounds = _window.ClientBounds;

                _renderTarget.Dispose();
                _renderTarget = new RenderTarget2D(_graphicsDevice, Bounds.Width, Bounds.Height);

                _boundsUpdatePending = false;
            }
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e) {
            _boundsUpdatePending = true;
        }
    }
}