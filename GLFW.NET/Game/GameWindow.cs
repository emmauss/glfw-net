﻿#region MIT License

// GameWindow.cs is distributed under the MIT License (MIT)
// 
// Copyright (c) 2018, Eric Freed
//   
// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// GameWindow.cs created on 06/11/2018

#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace GLFW.Game
{
	/// <summary>
	///     Provides a simplified interface for creating and using a GLFW window with properties, events, etc.
	/// </summary>
	/// <seealso cref="Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid" />
	public class GameWindow : SafeHandleZeroOrMinusOneIsInvalid
	{
		#region Fields and Constants

		/// <summary>
		///     The window instance this object wraps.
		/// </summary>
		protected Window Window;

		private string _title;

		#endregion

		#region Properties

		/// <summary>
		///     Gets or sets the size and location of the window including its non-client elements (borders, title bar, etc.), in
		///     screen coordinates.
		/// </summary>
		/// <value>
		///     A <see cref="Rectangle" /> in screen coordinates relative to the parent control that represents the size and
		///     location of the control including its nonclient elements.
		/// </value>
		public Rectangle Bounds
		{
			get => new Rectangle(Position, Size);
			set
			{
				Size = value.Size;
				Position = value.Location;
			}
		}

		/// <summary>
		///     Gets the size and location of the client area of the window, in screen coordinates.
		/// </summary>
		/// <value>
		///     A <see cref="Rectangle" /> in screen coordinates that represents the size and location of the window's client area.
		/// </value>
		public Rectangle ClientBounds
		{
			get => new Rectangle(Position, ClientSize);
			set
			{
				Glfw.SetWindowPosition(Window, value.X, value.Y);
				Glfw.SetWindowSize(Window, value.Width, value.Height);
			}
		}

		/// <summary>
		///     Gets or sets the size of the client area of the window, in screen coordinates.
		/// </summary>
		/// <value>
		///     A <see cref="System.Drawing.Size" /> in screen coordinates that represents the size of the window's client area.
		/// </value>
		public Size ClientSize
		{
			get
			{
				Glfw.GetWindowSize(Window, out var width, out var height);
				return new Size(width, height);
			}
			set => Glfw.SetWindowSize(Window, value.Width, value.Height);
		}

		/// <summary>
		///     Gets or sets a string to the system clipboard.
		/// </summary>
		/// <value>
		///     The clipboard string.
		/// </value>
		public string Clipboard
		{
			get => Glfw.GetClipboardString(Window);
			set => Glfw.SetClipboardString(Window, value);
		}

		/// <summary>
		///     Gets or sets the behavior of the mouse cursor.
		/// </summary>
		/// <value>
		///     The cursor mode.
		/// </value>
		public CursorMode CursorMode
		{
			get => (CursorMode) Glfw.GetInputMode(Window, InputMode.Cursor);
			set => Glfw.SetInputMode(Window, InputMode.Cursor, (int) value);
		}

		/// <summary>
		///     Gets the underlying pointer used by GLFW for this window instance.
		/// </summary>
		/// <value>
		///     The GLFW window handle.
		/// </value>
		public IntPtr Handle => handle;

		/// <summary>
		///     Gets the Window's HWND for this window.
		///     <para>WARNING: Windows only.</para>
		/// </summary>
		/// <value>
		///     The HWND pointer.
		/// </value>
		public IntPtr Hwnd
		{
			get
			{
				try { return Native.GetWin32Window(Window); }
				catch (Exception) { return IntPtr.Zero; }
			}
		}

		/// <summary>
		///     Gets a value indicating whether this instance is closing.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is closing; otherwise, <c>false</c>.
		/// </value>
		public bool IsClosing => Glfw.WindowShouldClose(Window);

		/// <summary>
		///     Gets a value indicating whether this instance is decorated.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is decorated; otherwise, <c>false</c>.
		/// </value>
		public bool IsDecorated => Glfw.GetWindowAttribute(Window, WindowAttribute.Decorated);

		/// <summary>
		///     Gets a value indicating whether this instance is floating (top-most, always-on-top).
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is floating; otherwise, <c>false</c>.
		/// </value>
		public bool IsFloating => Glfw.GetWindowAttribute(Window, WindowAttribute.Floating);

		/// <summary>
		///     Gets a value indicating whether this instance is focused.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is focused; otherwise, <c>false</c>.
		/// </value>
		public bool IsFocused => Glfw.GetWindowAttribute(Window, WindowAttribute.Focused);

		/// <summary>
		///     Gets a value indicating whether this instance is resizable.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is resizable; otherwise, <c>false</c>.
		/// </value>
		public bool IsResizable => Glfw.GetWindowAttribute(Window, WindowAttribute.Resizable);

		/// <summary>
		///     Gets or sets a value indicating whether this <see cref="GameWindow" /> is maximized.
		///     <para>Has no effect on fullscreen windows.</para>
		/// </summary>
		/// <value>
		///     <c>true</c> if maximized; otherwise, <c>false</c>.
		/// </value>
		public bool Maximized
		{
			get => Glfw.GetWindowAttribute(Window, WindowAttribute.Maximized);
			set
			{
				if (value)
					Glfw.MaximizeWindow(Window);
				else
					Glfw.RestoreWindow(Window);
			}
		}

		/// <summary>
		///     Gets or sets a value indicating whether this <see cref="GameWindow" /> is minimized.
		///     <para>If window is already minimized, does nothing.</para>
		/// </summary>
		/// <value>
		///     <c>true</c> if minimized; otherwise, <c>false</c>.
		/// </value>
		public bool Minimized
		{
			get => Glfw.GetWindowAttribute(Window, WindowAttribute.AutoIconify);
			set
			{
				if (value)
					Glfw.IconifyWindow(Window);
				else
					Glfw.RestoreWindow(Window);
			}
		}

		/// <summary>
		///     Gets the monitor this window is fullscreen on.
		///     <para>Returns <see cref="GLFW.Monitor.None" /> if window is not fullscreen.</para>
		/// </summary>
		/// <value>
		///     The monitor.
		/// </value>
		public Monitor Monitor => Glfw.GetWindowMonitor(Window);

		/// <summary>
		///     Gets or sets the mouse position in screen-coordinates relative to the client area of the window.
		/// </summary>
		/// <value>
		///     The mouse position.
		/// </value>
		public Point MousePosition
		{
			get
			{
				Glfw.GetCursorPosition(Window, out var x, out var y);
				return new Point(Convert.ToInt32(x), Convert.ToInt32(y));
			}
			set => Glfw.SetCursorPosition(Window, value.X, value.Y);
		}

		/// <summary>
		///     Gets or sets the position of the window in screen coordinates, including border, titlebar, etc..
		/// </summary>
		/// <value>
		///     The position.
		/// </value>
		public Point Position
		{
			get
			{
				Glfw.GetWindowPosition(Window, out var x, out var y);
				Glfw.GetWindowFrameSize(Window, out var l, out var t, out var dummy1, out var dummy2);
				return new Point(x - l, y - t);
			}
			set
			{
				Glfw.GetWindowFrameSize(Window, out var l, out var t, out var dummy1, out var dummy2);
				Glfw.SetWindowPosition(Window, value.X + l, value.Y + t);
			}
		}

		/// <summary>
		///     Gets or sets the size of the window, in screen coordinates, including border, titlebar, etc.
		///     <para>
		///         BUG: On Windows, as of GLFW 3.2.1, the frame size values may be incorrect, resulting in incorrect values for
		///         this property.
		///     </para>
		///     <para>
		///         This is due to how the values are retrieved from the OS by the underlying library. Typically the values will
		///         be larger than expected.
		///     </para>
		/// </summary>
		/// <value>
		///     A <see cref="System.Drawing.Size" /> in screen coordinates that represents the size of the window.
		/// </value>
		public Size Size
		{
			get
			{
				Glfw.GetWindowSize(Window, out var width, out var height);
				Glfw.GetWindowFrameSize(Window, out var l, out var t, out var r, out var b);
				return new Size(width + l + r, height + t + b);
			}
			set
			{
				Glfw.GetWindowFrameSize(Window, out var l, out var t, out var r, out var b);
				Glfw.SetWindowSize(Window, value.Width - l - r, value.Height - t - b);
			}
		}

		/// <summary>
		///     Sets the sticky keys input mode.
		///     <para>
		///         Set to <c>true</c> to enable sticky keys, or <c>false</c> to disable it. If sticky keys are enabled, a key
		///         press will ensure that <see cref="Glfw.GetKey" /> returns <see cref="InputState.Press" /> the next time it is
		///         called even if the key had been released before the call. This is useful when you are only interested in
		///         whether keys have been pressed but not when or in which order.
		///     </para>
		/// </summary>
		public bool StickyKeys
		{
			get => Glfw.GetInputMode(Window, InputMode.StickyKeys) == (int) Constants.True;
			set => Glfw.SetInputMode(Window, InputMode.StickyKeys,
				value ? (int) Constants.True : (int) Constants.False);
		}

		/// <summary>
		///     Gets or sets the sticky mouse button input mode.
		///     <para>
		///         Set to <c>true</c> to enable sticky mouse buttons, or <c>false</c> to disable it. If sticky mouse buttons are
		///         enabled, a mouse button press will ensure that <see cref="Glfw.GetMouseButton" /> returns
		///         <see cref="InputState.Press" /> the next time it is called even if the mouse button had been released before
		///         the call. This is useful when you are only interested in whether mouse buttons have been pressed but not when
		///         or in which order.
		///     </para>
		/// </summary>
		public bool StickyMouseButtons
		{
			get => Glfw.GetInputMode(Window, InputMode.StickyMouseButton) == (int) Constants.True;
			set => Glfw.SetInputMode(Window, InputMode.StickyMouseButton,
				value ? (int) Constants.True : (int) Constants.False);
		}

		/// <summary>
		///     Gets or sets the window title or caption.
		/// </summary>
		/// <value>
		///     The title.
		/// </value>
		public string Title
		{
			get => _title;
			set
			{
				_title = value;
				Glfw.SetWindowTitle(Window, value);
			}
		}

		/// <summary>
		///     Gets or sets a user-defined pointer for GLFW to retain for this instance.
		/// </summary>
		/// <value>
		///     The user-defined pointer.
		/// </value>
		public IntPtr UserPointer
		{
			get => Glfw.GetWindowUserPointer(Window);
			set => Glfw.SetWindowUserPointer(Window, value);
		}

		/// <summary>
		///     Gets the video mode for the monitor this window is fullscreen on.
		///     <para>If window is not fullscreen, returns the <see cref="GLFW.VideoMode" /> for the primary monitor.</para>
		/// </summary>
		/// <value>
		///     The video mode.
		/// </value>
		public VideoMode VideoMode
		{
			get
			{
				var monitor = Monitor;
				return Glfw.GetVideoMode(monitor == Monitor.None ? Glfw.PrimaryMonitor : monitor);
			}
		}

		/// <summary>
		///     Gets or sets a value indicating whether this <see cref="GameWindow" /> is visible.
		/// </summary>
		/// <value>
		///     <c>true</c> if visible; otherwise, <c>false</c>.
		/// </value>
		public bool Visible
		{
			get => Glfw.GetWindowAttribute(Window, WindowAttribute.Visible);
			set
			{
				if (value)
					Glfw.ShowWindow(Window);
				else
					Glfw.HideWindow(Window);
			}
		}

		#region Operators

		/// <summary>
		///     Performs an implicit conversion from <see cref="GameWindow" /> to <see cref="GLFW.Window" />.
		/// </summary>
		/// <param name="gameWindow">The game window.</param>
		/// <returns>
		///     The result of the conversion.
		/// </returns>
		public static implicit operator Window(GameWindow gameWindow) => gameWindow.Window;

		/// <summary>
		///     Performs an implicit conversion from <see cref="GameWindow" /> to <see cref="IntPtr" />.
		/// </summary>
		/// <param name="gameWindow">The game window.</param>
		/// <returns>
		///     The result of the conversion.
		/// </returns>
		public static implicit operator IntPtr(GameWindow gameWindow) => gameWindow.Window;

		#endregion

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GameWindow"/> class.
		/// </summary>
		public GameWindow() : this(640, 480, string.Empty, Monitor.None, Window.None) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="GameWindow" /> class.
		/// </summary>
		/// <param name="width">The desired width, in screen coordinates, of the window. This must be greater than zero.</param>
		/// <param name="height">The desired height, in screen coordinates, of the window. This must be greater than zero.</param>
		/// <param name="title">The initial window title.</param>
		public GameWindow(int width, int height, string title) :
			this(width, height, title, Monitor.None, Window.None) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="GameWindow" /> class.
		/// </summary>
		/// <param name="width">The desired width, in screen coordinates, of the window. This must be greater than zero.</param>
		/// <param name="height">The desired height, in screen coordinates, of the window. This must be greater than zero.</param>
		/// <param name="title">The initial window title.</param>
		/// <param name="monitor">The monitor to use for full screen mode, or <see cref="GLFW.Monitor.None" /> for windowed mode.</param>
		/// <param name="share">
		///     A window instance whose context to share resources with, or <see cref="GLFW.Window.None" /> to not share
		///     resources..
		/// </param>
		public GameWindow(int width, int height, string title, Monitor monitor, Window share) : base(true)
		{
			_title = title;
			Window = Glfw.CreateWindow(width, height, title, monitor, share);
			SetHandle(Window);
			MakeCurrent();
			BindCallbacks();
		}

		#endregion

		#region Methods

		/// <summary>
		///     Centers the on window on the screen.
		///     <para>Has no effect on fullscreen or maximized windows.</para>
		/// </summary>
		public void CenterOnScreen()
		{
			if (Maximized)
				return;
			var monitor = Monitor == Monitor.None ? Glfw.PrimaryMonitor : Monitor;
			var videoMode = Glfw.GetVideoMode(monitor);
			var size = Size;
			Position = new Point((videoMode.Width - size.Width) / 2, (videoMode.Height - size.Height) / 2);
		}

		/// <summary>
		///     Closes this instance.
		///     <para>This invalidates the window, but does not free its resouces.</para>
		/// </summary>
		public new void Close()
		{
			Glfw.SetWindowShouldClose(Window, true);
			OnClosing();
			base.Close();
		}

		/// <summary>
		///     Focuses this form to receive input and events.
		/// </summary>
		public void Focus() => Glfw.FocusWindow(Window);

		/// <summary>
		///     Sets the window fullscreen on the primary monitor.
		/// </summary>
		public void Fullscreen() => Fullscreen(Glfw.PrimaryMonitor);

		/// <summary>
		///     Sets the window fullscreen on the specified monitor.
		/// </summary>
		/// <param name="monitor">The monitor to display the window fullscreen.</param>
		public void Fullscreen(Monitor monitor) => Glfw.SetWindowMonitor(Window, monitor, 0, 0, 0, 0, -1);

		/// <summary>
		/// Makes window and its context the current.
		/// </summary>
		public void MakeCurrent() => Glfw.MakeContextCurrent(Window);

		/// <summary>
		///     Maximizes this window to fill the screen.
		///     <para>Has no effect if window is already maximized.</para>
		/// </summary>
		public void Maximize() => Glfw.MaximizeWindow(Window);

		/// <summary>
		///     Minimizes this window.
		///     <para>Has no effect if window is already minimized.</para>
		/// </summary>
		public void Minimize() => Glfw.IconifyWindow(Window);

		/// <summary>
		///     Restores a minimized window to its previous state.
		///     <para>Has no effect if window was already restored.</para>
		/// </summary>
		public void Restore() => Glfw.RestoreWindow(Window);

		/// <summary>
		///     Sets the aspect ratio to maintain for the window.
		///     <para>This function is ignored for fullscreen windows.</para>
		/// </summary>
		/// <param name="numerator">The numerator of the desired aspect ratio.</param>
		/// <param name="denominator">The denominator of the desired aspect ratio.</param>
		public void SetAspectRatio(int numerator, int denominator) =>
			Glfw.SetWindowAspectRatio(Window, numerator, denominator);

		/// <summary>
		///     Sets the icon(s) used for the titlebar, taskbar, etc.
		///     <para>Standard sizes are 16x16, 32x32, and 48x48.</para>
		/// </summary>
		/// <param name="images">One or more images to set as an icon.</param>
		public void SetIcons(params Image[] images) => Glfw.SetWindowIcon(Window, images.Length, images);

		/// <summary>
		///     Sets the window monitor.
		///     <para>
		///         If <paramref name="monitor" /> is not <see cref="GLFW.Monitor.None" />, the window will be fullscreened and
		///         dimensions ignored.
		///     </para>
		/// </summary>
		/// <param name="monitor">The desired monitor, or <see cref="GLFW.Monitor.None" /> to set windowed mode.</param>
		/// <param name="x">The desired x-coordinate of the upper-left corner of the client area.</param>
		/// <param name="y">The desired y-coordinate of the upper-left corner of the client area.</param>
		/// <param name="width">The desired width, in screen coordinates, of the client area or video mode.</param>
		/// <param name="height">The desired height, in screen coordinates, of the client area or video mode.</param>
		public void SetMonitor(Monitor monitor, int x, int y, int width, int height) =>
			SetMonitor(monitor, x, y, width, height, (int) Constants.Default);

		/// <summary>
		///     Sets the window monitor.
		///     <para>
		///         If <paramref name="monitor" /> is not <see cref="GLFW.Monitor.None" />, the window will be fullscreened and
		///         dimensions ignored.
		///     </para>
		/// </summary>
		/// <param name="monitor">The desired monitor, or <see cref="GLFW.Monitor.None" /> to set windowed mode.</param>
		/// <param name="x">The desired x-coordinate of the upper-left corner of the client area.</param>
		/// <param name="y">The desired y-coordinate of the upper-left corner of the client area.</param>
		/// <param name="width">The desired width, in screen coordinates, of the client area or video mode.</param>
		/// <param name="height">The desired height, in screen coordinates, of the client area or video mode.</param>
		/// <param name="refreshRate">The desired refresh rate, in Hz, of the video mode, or <see cref="Constants.Default" />.</param>
		public void SetMonitor(Monitor monitor, int x, int y, int width, int height, int refreshRate) =>
			Glfw.SetWindowMonitor(Window, monitor, x, y, width, height, refreshRate);

		/// <summary>
		///     Sets the imits of the client size  area of the window.
		/// </summary>
		/// <param name="minSize">The minimum size of the client area.</param>
		/// <param name="maxSize">The maximum size of the client area.</param>
		public void SetSizeLimits(Size minSize, Size maxSize) =>
			SetSizeLimits(minSize.Width, minSize.Height, maxSize.Width, maxSize.Height);

		/// <summary>
		///     Sets the imits of the client size  area of the window.
		/// </summary>
		/// <param name="minWidth">The minimum width of the client area.</param>
		/// <param name="minHeight">The minimum height of the client area.</param>
		/// <param name="maxWidth">The maximum width of the client area.</param>
		/// <param name="maxHeight">The maximum height of the client area.</param>
		public void SetSizeLimits(int minWidth, int minHeight, int maxWidth, int maxHeight)
		{
			Glfw.SetWindowSizeLimits(Window, minWidth, minHeight, maxWidth, maxHeight);
		}

		/// <summary>
		///     Swaps the front and back buffers when rendering with OpenGL or OpenGL ES.
		/// </summary>
		public void SwapBuffers() => Glfw.SwapBuffers(Window);

		/// <summary>
		///     Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing">
		///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
		///     unmanaged resources.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			Disposed?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		///     Releases the internal GLFW handle.
		/// </summary>
		/// <returns><c>true</c> if handle was released successfully, otherwise <c>false</c>.</returns>
		protected override bool ReleaseHandle()
		{
			try
			{
				Glfw.DestroyWindow(Window);
				return true;
			}
			catch (Exception) { return false; }
		}

		private void BindCallbacks()
		{
			Glfw.SetWindowPositionCallback(Window, (_, x, y) => OnPositionChanged(x, y));
			Glfw.SetWindowSizeCallback(Window, (_, w, h) => OnSizeChanged(w, h));
			Glfw.SetWindowFocusCallback(Window, (_, focusing) => OnFocusChanged(focusing));
			Glfw.SetCloseCallback(Window, _ => OnClosing());
			Glfw.SetDropCallback(Window, (_, count, arrayPtr) => OnFileDrop(count, arrayPtr));
			Glfw.SetCursorPositionCallback(Window, (_, x, y) => OnMouseMove(x, y));
			Glfw.SetCursorEnterCallback(Window, (_, entering) => OnMouseEnter(entering));
			Glfw.SetMouseButtonCallback(Window, (_, button, state, mod) => OnMouseButton(button, state, mod));
			Glfw.SetScrollCallback(Window, (_, x, y) => OnMouseScroll(x, y));
			Glfw.SetCharModsCallback(Window, (_, cp, mods) => OnCharacterInput(cp, mods));
			Glfw.SetFramebufferSizeCallback(Window, (_, w, h) => OnFramebufferSizeChanged(w, h));
			Glfw.SetWindowRefreshCallback(Window, _ => Refreshed?.Invoke(this, EventArgs.Empty));
			Glfw.SetKeyCallback(Window, (_, key, code, state, mods) => OnKey(key, code, state, mods));
		}

		private void OnFileDrop(int count, IntPtr pointer)
		{
			var filenames = new string[count];
			var offset = 0;
			for (var i = 0; i < count; i++, offset += IntPtr.Size)
			{
				var ptr = new IntPtr(Marshal.ReadInt32(pointer + offset));
				filenames[i] = Util.PtrToStringUTF8(ptr);
			}

			OnFileDrop(filenames);
		}

		#endregion

		#region Delegates and Events

		/// <summary>
		///     Occurs when the window receives character input.
		/// </summary>
		public event EventHandler<CharEventArgs> CharacterInput;

		/// <summary>
		///     Occurs when the window is closed.
		/// </summary>
		public event EventHandler Closed;

		/// <summary>
		///     Occurs when the form is closing, and provides subscribers means of canceling the action..
		/// </summary>
		public event CancelEventHandler Closing;

		/// <summary>
		///     Occurs when the window is disposed.
		/// </summary>
		public event EventHandler Disposed;

		/// <summary>
		///     Occurs when files are dropped onto the window client area with a drag-drop event.
		/// </summary>
		public event EventHandler<FileDropEventArgs> FileDrop;

		/// <summary>
		///     Occurs when the window gains or loses focus.
		/// </summary>
		public event EventHandler FocusChanged;

		/// <summary>
		///     Occurs when the size of the internal framebuffer is changed.
		/// </summary>
		public event EventHandler<SizeChangeEventArgs> FramebufferSizeChanged;

		/// <summary>
		///     Occurs when a key is pressed, released, or repeated.
		/// </summary>
		public event EventHandler<KeyEventArgs> KeyAction;

		/// <summary>
		///     Occurs when a key is pressed.
		/// </summary>
		public event EventHandler<KeyEventArgs> KeyPress;

		/// <summary>
		///     Occurs when a key is released.
		/// </summary>
		public event EventHandler<KeyEventArgs> KeyRelease;

		/// <summary>
		///     Occurs when a key is held long enough to raise a repeat event.
		/// </summary>
		public event EventHandler<KeyEventArgs> KeyRepeat;

		/// <summary>
		///     Occurs when a mouse button is pressed or released.
		/// </summary>
		public event EventHandler<MouseButtonEventArgs> MouseButton;

		/// <summary>
		///     Occurs when the mouse cursor enters the client area of the window.
		/// </summary>
		public event EventHandler MouseEnter;

		/// <summary>
		///     Occurs when the mouse cursor leaves the client area of the window.
		/// </summary>
		public event EventHandler MouseLeave;

		/// <summary>
		///     Occurs when mouse cursor is moved.
		/// </summary>
		public event EventHandler<MouseMoveEventArgs> MouseMoved;

		/// <summary>
		///     Occurs when mouse is scrolled.
		/// </summary>
		public event EventHandler<MouseMoveEventArgs> MouseScroll;

		/// <summary>
		///     Occurs when position of the <see cref="GameWindow" /> is changed.
		/// </summary>
		public event EventHandler PositionChanged;

		/// <summary>
		///     Occurs when window is refreshed.
		/// </summary>
		public event EventHandler Refreshed;

		/// <summary>
		///     Occurs when size of the <see cref="GameWindow" /> is changed.
		/// </summary>
		public event EventHandler<SizeChangeEventArgs> SizeChanged;

		/// <summary>
		///     Raises the <see cref="CharacterInput" /> event.
		/// </summary>
		/// <param name="codePoint">The Unicode code point.</param>
		/// <param name="mods">The modifier keys present.</param>
		protected virtual void OnCharacterInput(uint codePoint, ModiferKeys mods)
		{
			CharacterInput?.Invoke(this, new CharEventArgs(codePoint, mods));
		}

		/// <summary>
		///     Raises the <see cref="Closed" /> event.
		/// </summary>
		protected virtual void OnClosed() => Closed?.Invoke(this, EventArgs.Empty);

		/// <summary>
		///     Raises the <see cref="Closing" /> event.
		/// </summary>
		protected virtual void OnClosing()
		{
			var args = new CancelEventArgs();
			Closing?.Invoke(this, args);
			if (args.Cancel)
			{
				Glfw.SetWindowShouldClose(Window, false);
			}
			else
			{
				base.Close();
				OnClosed();
			}
		}

		/// <summary>
		///     Raises the <see cref="FileDrop" /> event.
		/// </summary>
		/// <param name="filenames">The filenames of the dropped files.</param>
		protected virtual void OnFileDrop(string[] filenames)
		{
			FileDrop?.Invoke(this, new FileDropEventArgs(filenames));
		}

		/// <summary>
		///     Raises the <see cref="FocusChanged" /> event.
		/// </summary>
		/// <param name="focusing"><c>true</c> if window is gaining focus, otherwise <c>false</c>.</param>
		protected virtual void OnFocusChanged(bool focusing) => FocusChanged?.Invoke(this, EventArgs.Empty);

		/// <summary>
		///     Raises the <see cref="FramebufferSizeChanged" /> event.
		/// </summary>
		/// <param name="width">The new width.</param>
		/// <param name="height">The new height.</param>
		protected virtual void OnFramebufferSizeChanged(int width, int height)
		{
			FramebufferSizeChanged?.Invoke(this, new SizeChangeEventArgs(new Size(width, height)));
		}

		/// <summary>
		///     Raises the appropriate key events.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="scanCode">The scan code.</param>
		/// <param name="state">The state of the key.</param>
		/// <param name="mods">The modifier keys.</param>
		/// <seealso cref="KeyPress" />
		/// <seealso cref="KeyRelease" />
		/// <seealso cref="KeyRepeat" />
		/// <seealso cref="KeyAction" />
		protected virtual void OnKey(Keys key, int scanCode, InputState state, ModiferKeys mods)
		{
			var args = new KeyEventArgs(key, scanCode, state, mods);
			if (state.HasFlag(InputState.Press))
				KeyPress?.Invoke(this, args);
			else if (state.HasFlag(InputState.Release))
				KeyRelease?.Invoke(this, args);
			else
				KeyRepeat?.Invoke(this, args);
			KeyAction?.Invoke(this, args);
		}

		/// <summary>
		///     Raises the <see cref="MouseButton" /> event.
		/// </summary>
		/// <param name="button">The mouse button.</param>
		/// <param name="state">The state of the mouse button.</param>
		/// <param name="modifiers">The modifier keys.</param>
		protected virtual void OnMouseButton(MouseButton button, InputState state, ModiferKeys modifiers)
		{
			MouseButton?.Invoke(this, new MouseButtonEventArgs(button, state, modifiers));
		}

		/// <summary>
		///     Raises the <see cref="MouseEnter" /> and <see cref="MouseLeave" /> events.
		/// </summary>
		/// <param name="entering"><c>true</c> if mouse is entering window, otherwise <c>false</c> if it is leaving.</param>
		protected virtual void OnMouseEnter(bool entering)
		{
			if (entering)
				MouseEnter?.Invoke(this, EventArgs.Empty);
			else
				MouseLeave?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		///     Raises the <see cref="MouseMoved" /> event.
		/// </summary>
		/// <param name="x">The new x-coordiinate of the mouse.</param>
		/// <param name="y">The new y-coordiinate of the mouse.</param>
		protected virtual void OnMouseMove(double x, double y) =>
			MouseMoved?.Invoke(this, new MouseMoveEventArgs(x, y));

		/// <summary>
		///     Raises the <see cref="MouseScroll" /> event.
		/// </summary>
		/// <param name="x">The amount of the scroll on the x-axis.</param>
		/// <param name="y">The amount of the scroll on the y-axis.</param>
		protected virtual void OnMouseScroll(double x, double y) =>
			MouseScroll?.Invoke(this, new MouseMoveEventArgs(x, y));

		/// <summary>
		///     Raises the <see cref="PositionChanged" /> event.
		/// </summary>
		/// <param name="x">The new position on the x-axis.</param>
		/// <param name="y">The new position on the y-axis.</param>
		protected virtual void OnPositionChanged(double x, double y) => PositionChanged?.Invoke(this, EventArgs.Empty);

		/// <summary>
		///     Raises the <see cref="SizeChanged" /> event.
		/// </summary>
		/// <param name="width">The new width.</param>
		/// <param name="height">The new height.</param>
		protected virtual void OnSizeChanged(int width, int height) =>
			SizeChanged?.Invoke(this, new SizeChangeEventArgs(new Size(width, height)));

		#endregion
	}
}