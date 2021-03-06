#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2013 Stefanos Apostolopoulos for the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using System.Diagnostics;
using OpenTK.Graphics;
using OpenTK.Input;

namespace OpenTK.Platform.SDL2
{
    class Sdl2Factory : IPlatformFactory
    {
        readonly IInputDriver2 InputDriver = new Sdl2InputDriver();
        bool disposed;

        /// <summary>
        /// Gets or sets a value indicating whether to use SDL2 fullscreen-desktop mode
        /// for fullscreen windows. When true, then GameWindow instances will not change
        /// DisplayDevice resolutions when going fullscreen. When false, fullscreen GameWindows
        /// will change the device resolution to match their size.
        /// </summary>
        /// <remarks>>
        /// This is a workaround for the lack of ChangeResolution support in SDL2.
        /// When and if this changes upstream, we should remove this code.
        /// </remarks>
        public static bool UseFullscreenDesktop { get; set; }

        public Sdl2Factory()
        {
            UseFullscreenDesktop = true;
        }

        #region IPlatformFactory implementation

        public INativeWindow CreateNativeWindow(int x, int y, int width, int height, string title, GraphicsMode mode, GameWindowFlags options, DisplayDevice device)
        {
            return new Sdl2NativeWindow(x, y, width, height, title, options, device);
        }

        public IDisplayDeviceDriver CreateDisplayDeviceDriver()
        {
            return new Sdl2DisplayDeviceDriver();
        }

        virtual public IGraphicsContext CreateGLContext(GraphicsMode mode, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
        {
            return new Sdl2GraphicsContext(mode, window, shareContext, major, minor, flags);
        }

        public IGraphicsContext CreateGLContext(ContextHandle handle, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
        {
            throw new NotImplementedException();
        }

        public GraphicsContext.GetCurrentContextDelegate CreateGetCurrentGraphicsContext()
        {
            return (GraphicsContext.GetCurrentContextDelegate)delegate
            {
                return Sdl2GraphicsContext.GetCurrentContext();
            };
        }

        public IGraphicsMode CreateGraphicsMode()
        {
            return new Sdl2GraphicsMode();
        }

        public IKeyboardDriver2 CreateKeyboardDriver()
        {
            return InputDriver.KeyboardDriver;
        }

        public IMouseDriver2 CreateMouseDriver()
        {
            return InputDriver.MouseDriver;
        }

        public IGamePadDriver CreateGamePadDriver()
        {
            return InputDriver.GamePadDriver;
        }

        #endregion

        #region IDisposable Members

        void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    Debug.Print("Disposing {0}", GetType());
                    InputDriver.Dispose();
                }
                else
                {
                    Debug.WriteLine("Sdl2Factory leaked, did you forget to call Dispose()?");
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Sdl2Factory()
        {
            Dispose(false);
        }

        #endregion
    }
}

