/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

namespace GameEngine
{
    /// <summary>
    /// 虚拟按键编码的枚举类型定义<br/>
    /// 这些按键编码既有对应于物件键盘上的按键编码，由底层系统推送，也有部分抽象概念的编码，由框架推送<br/>
    /// <br/>
    /// 虚拟按键的实际编码值，与当前运行的引擎有关<br/>
    /// 在 Unity 引擎中，虚拟按键编码与<see cref="UnityEngine.KeyCode"/>枚举类型一致<br/>
    /// </summary>
    public enum VirtualKeyCode
    {
        //
        // Summary:
        //     Not assigned (never returned as the result of a keystroke).
        None = 0,
        //
        // Summary:
        //     The backspace key.
        Backspace = UnityEngine.KeyCode.Backspace,
        //
        // Summary:
        //     The forward delete key.
        Delete = UnityEngine.KeyCode.Delete,
        //
        // Summary:
        //     The tab key.
        Tab = UnityEngine.KeyCode.Tab,
        //
        // Summary:
        //     The Clear key.
        Clear = UnityEngine.KeyCode.Clear,
        //
        // Summary:
        //     Return key.
        Return = UnityEngine.KeyCode.Return,
        //
        // Summary:
        //     Pause on PC machines.
        Pause = UnityEngine.KeyCode.Pause,
        //
        // Summary:
        //     Escape key.
        Escape = UnityEngine.KeyCode.Escape,
        //
        // Summary:
        //     Space key.
        Space = UnityEngine.KeyCode.Space,
        //
        // Summary:
        //     Numeric keypad 0.
        Keypad0 = UnityEngine.KeyCode.Keypad0,
        //
        // Summary:
        //     Numeric keypad 1.
        Keypad1 = UnityEngine.KeyCode.Keypad1,
        //
        // Summary:
        //     Numeric keypad 2.
        Keypad2 = UnityEngine.KeyCode.Keypad2,
        //
        // Summary:
        //     Numeric keypad 3.
        Keypad3 = UnityEngine.KeyCode.Keypad3,
        //
        // Summary:
        //     Numeric keypad 4.
        Keypad4 = UnityEngine.KeyCode.Keypad4,
        //
        // Summary:
        //     Numeric keypad 5.
        Keypad5 = UnityEngine.KeyCode.Keypad5,
        //
        // Summary:
        //     Numeric keypad 6.
        Keypad6 = UnityEngine.KeyCode.Keypad6,
        //
        // Summary:
        //     Numeric keypad 7.
        Keypad7 = UnityEngine.KeyCode.Keypad7,
        //
        // Summary:
        //     Numeric keypad 8.
        Keypad8 = UnityEngine.KeyCode.Keypad8,
        //
        // Summary:
        //     Numeric keypad 9.
        Keypad9 = UnityEngine.KeyCode.Keypad9,
        //
        // Summary:
        //     Numeric keypad '.'.
        KeypadPeriod = UnityEngine.KeyCode.KeypadPeriod,
        //
        // Summary:
        //     Numeric keypad '/'.
        KeypadDivide = UnityEngine.KeyCode.KeypadDivide,
        //
        // Summary:
        //     Numeric keypad '*'.
        KeypadMultiply = UnityEngine.KeyCode.KeypadMultiply,
        //
        // Summary:
        //     Numeric keypad '-'.
        KeypadMinus = UnityEngine.KeyCode.KeypadMinus,
        //
        // Summary:
        //     Numeric keypad '+'.
        KeypadPlus = UnityEngine.KeyCode.KeypadPlus,
        //
        // Summary:
        //     Numeric keypad Enter.
        KeypadEnter = UnityEngine.KeyCode.KeypadEnter,
        //
        // Summary:
        //     Numeric keypad '='.
        KeypadEquals = UnityEngine.KeyCode.KeypadEquals,
        //
        // Summary:
        //     Up arrow key.
        UpArrow = UnityEngine.KeyCode.UpArrow,
        //
        // Summary:
        //     Down arrow key.
        DownArrow = UnityEngine.KeyCode.DownArrow,
        //
        // Summary:
        //     Right arrow key.
        RightArrow = UnityEngine.KeyCode.RightArrow,
        //
        // Summary:
        //     Left arrow key.
        LeftArrow = UnityEngine.KeyCode.LeftArrow,
        //
        // Summary:
        //     Insert key key.
        Insert = UnityEngine.KeyCode.Insert,
        //
        // Summary:
        //     Home key.
        Home = UnityEngine.KeyCode.Home,
        //
        // Summary:
        //     End key.
        End = UnityEngine.KeyCode.End,
        //
        // Summary:
        //     Page up.
        PageUp = UnityEngine.KeyCode.PageUp,
        //
        // Summary:
        //     Page down.
        PageDown = UnityEngine.KeyCode.PageDown,
        //
        // Summary:
        //     F1 function key.
        F1 = UnityEngine.KeyCode.F1,
        //
        // Summary:
        //     F2 function key.
        F2 = UnityEngine.KeyCode.F2,
        //
        // Summary:
        //     F3 function key.
        F3 = UnityEngine.KeyCode.F3,
        //
        // Summary:
        //     F4 function key.
        F4 = UnityEngine.KeyCode.F4,
        //
        // Summary:
        //     F5 function key.
        F5 = UnityEngine.KeyCode.F5,
        //
        // Summary:
        //     F6 function key.
        F6 = UnityEngine.KeyCode.F6,
        //
        // Summary:
        //     F7 function key.
        F7 = UnityEngine.KeyCode.F7,
        //
        // Summary:
        //     F8 function key.
        F8 = UnityEngine.KeyCode.F8,
        //
        // Summary:
        //     F9 function key.
        F9 = UnityEngine.KeyCode.F9,
        //
        // Summary:
        //     F10 function key.
        F10 = UnityEngine.KeyCode.F10,
        //
        // Summary:
        //     F11 function key.
        F11 = UnityEngine.KeyCode.F11,
        //
        // Summary:
        //     F12 function key.
        F12 = UnityEngine.KeyCode.F12,
        //
        // Summary:
        //     F13 function key.
        F13 = UnityEngine.KeyCode.F13,
        //
        // Summary:
        //     F14 function key.
        F14 = UnityEngine.KeyCode.F14,
        //
        // Summary:
        //     F15 function key.
        F15 = UnityEngine.KeyCode.F15,
        //
        // Summary:
        //     The '0' key on the top of the alphanumeric keyboard.
        Alpha0 = UnityEngine.KeyCode.Alpha0,
        //
        // Summary:
        //     The '1' key on the top of the alphanumeric keyboard.
        Alpha1 = UnityEngine.KeyCode.Alpha1,
        //
        // Summary:
        //     The '2' key on the top of the alphanumeric keyboard.
        Alpha2 = UnityEngine.KeyCode.Alpha2,
        //
        // Summary:
        //     The '3' key on the top of the alphanumeric keyboard.
        Alpha3 = UnityEngine.KeyCode.Alpha3,
        //
        // Summary:
        //     The '4' key on the top of the alphanumeric keyboard.
        Alpha4 = UnityEngine.KeyCode.Alpha4,
        //
        // Summary:
        //     The '5' key on the top of the alphanumeric keyboard.
        Alpha5 = UnityEngine.KeyCode.Alpha5,
        //
        // Summary:
        //     The '6' key on the top of the alphanumeric keyboard.
        Alpha6 = UnityEngine.KeyCode.Alpha6,
        //
        // Summary:
        //     The '7' key on the top of the alphanumeric keyboard.
        Alpha7 = UnityEngine.KeyCode.Alpha7,
        //
        // Summary:
        //     The '8' key on the top of the alphanumeric keyboard.
        Alpha8 = UnityEngine.KeyCode.Alpha8,
        //
        // Summary:
        //     The '9' key on the top of the alphanumeric keyboard.
        Alpha9 = UnityEngine.KeyCode.Alpha9,
        //
        // Summary:
        //     Exclamation mark key '!'. Deprecated if "Use Physical Keys" is enabled in instead.
        Exclaim = UnityEngine.KeyCode.Exclaim,
        //
        // Summary:
        //     Double quote key '"'. Deprecated if "Use Physical Keys" is enabled in instead.
        DoubleQuote = UnityEngine.KeyCode.DoubleQuote,
        //
        // Summary:
        //     Hash key '#'. Deprecated if "Use Physical Keys" is enabled in instead.
        Hash = UnityEngine.KeyCode.Hash,
        //
        // Summary:
        //     Dollar sign key '$'. Deprecated if "Use Physical Keys" is enabled in instead.
        Dollar = UnityEngine.KeyCode.Dollar,
        //
        // Summary:
        //     Percent '%' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Percent = UnityEngine.KeyCode.Percent,
        //
        // Summary:
        //     Ampersand key '&'. Deprecated if "Use Physical Keys" is enabled in instead.
        Ampersand = UnityEngine.KeyCode.Ampersand,
        //
        // Summary:
        //     Quote key '.
        Quote = UnityEngine.KeyCode.Quote,
        //
        // Summary:
        //     Left Parenthesis key '('. Deprecated if "Use Physical Keys" is enabled in instead.
        LeftParen = UnityEngine.KeyCode.LeftParen,
        //
        // Summary:
        //     Right Parenthesis key ')'. Deprecated if "Use Physical Keys" is enabled in instead.
        RightParen = UnityEngine.KeyCode.RightParen,
        //
        // Summary:
        //     Asterisk key '*'. Deprecated if "Use Physical Keys" is enabled in instead.
        Asterisk = UnityEngine.KeyCode.Asterisk,
        //
        // Summary:
        //     Plus key '+'. Deprecated if "Use Physical Keys" is enabled in instead.
        Plus = UnityEngine.KeyCode.Plus,
        //
        // Summary:
        //     Comma ',' key.
        Comma = UnityEngine.KeyCode.Comma,
        //
        // Summary:
        //     Minus '-' key.
        Minus = UnityEngine.KeyCode.Minus,
        //
        // Summary:
        //     Period '.' key.
        Period = UnityEngine.KeyCode.Period,
        //
        // Summary:
        //     Slash '/' key.
        Slash = UnityEngine.KeyCode.Slash,
        //
        // Summary:
        //     Colon ':' key.Deprecated if "Use Physical Keys" is enabled in instead.
        Colon = UnityEngine.KeyCode.Colon,
        //
        // Summary:
        //     Semicolon ';' key.
        Semicolon = UnityEngine.KeyCode.Semicolon,
        //
        // Summary:
        //     Less than '<' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Less = UnityEngine.KeyCode.Less,
        //
        // Summary:
        //     Equals '=' key.
        Equals = UnityEngine.KeyCode.Equals,
        //
        // Summary:
        //     Greater than '>' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Greater = UnityEngine.KeyCode.Greater,
        //
        // Summary:
        //     Question mark '?' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Question = UnityEngine.KeyCode.Question,
        //
        // Summary:
        //     At key '@'. Deprecated if "Use Physical Keys" is enabled in instead.
        At = UnityEngine.KeyCode.At,
        //
        // Summary:
        //     Left square bracket key '['.
        LeftBracket = UnityEngine.KeyCode.LeftBracket,
        //
        // Summary:
        //     Backslash key '\'.
        Backslash = UnityEngine.KeyCode.Backslash,
        //
        // Summary:
        //     Right square bracket key ']'.
        RightBracket = UnityEngine.KeyCode.RightBracket,
        //
        // Summary:
        //     Caret key '^'. Deprecated if "Use Physical Keys" is enabled in instead.
        Caret = UnityEngine.KeyCode.Caret,
        //
        // Summary:
        //     Underscore '_' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Underscore = UnityEngine.KeyCode.Underscore,
        //
        // Summary:
        //     Back quote key '`'.
        BackQuote = UnityEngine.KeyCode.BackQuote,
        //
        // Summary:
        //     'a' key.
        A = UnityEngine.KeyCode.A,
        //
        // Summary:
        //     'b' key.
        B = UnityEngine.KeyCode.B,
        //
        // Summary:
        //     'c' key.
        C = UnityEngine.KeyCode.C,
        //
        // Summary:
        //     'd' key.
        D = UnityEngine.KeyCode.D,
        //
        // Summary:
        //     'e' key.
        E = UnityEngine.KeyCode.E,
        //
        // Summary:
        //     'f' key.
        F = UnityEngine.KeyCode.F,
        //
        // Summary:
        //     'g' key.
        G = UnityEngine.KeyCode.G,
        //
        // Summary:
        //     'h' key.
        H = UnityEngine.KeyCode.H,
        //
        // Summary:
        //     'i' key.
        I = UnityEngine.KeyCode.I,
        //
        // Summary:
        //     'j' key.
        J = UnityEngine.KeyCode.J,
        //
        // Summary:
        //     'k' key.
        K = UnityEngine.KeyCode.K,
        //
        // Summary:
        //     'l' key.
        L = UnityEngine.KeyCode.L,
        //
        // Summary:
        //     'm' key.
        M = UnityEngine.KeyCode.M,
        //
        // Summary:
        //     'n' key.
        N = UnityEngine.KeyCode.N,
        //
        // Summary:
        //     'o' key.
        O = UnityEngine.KeyCode.O,
        //
        // Summary:
        //     'p' key.
        P = UnityEngine.KeyCode.P,
        //
        // Summary:
        //     'q' key.
        Q = UnityEngine.KeyCode.Q,
        //
        // Summary:
        //     'r' key.
        R = UnityEngine.KeyCode.R,
        //
        // Summary:
        //     's' key.
        S = UnityEngine.KeyCode.S,
        //
        // Summary:
        //     't' key.
        T = UnityEngine.KeyCode.T,
        //
        // Summary:
        //     'u' key.
        U = UnityEngine.KeyCode.U,
        //
        // Summary:
        //     'v' key.
        V = UnityEngine.KeyCode.V,
        //
        // Summary:
        //     'w' key.
        W = UnityEngine.KeyCode.W,
        //
        // Summary:
        //     'x' key.
        X = UnityEngine.KeyCode.X,
        //
        // Summary:
        //     'y' key.
        Y = UnityEngine.KeyCode.Y,
        //
        // Summary:
        //     'z' key.
        Z = UnityEngine.KeyCode.Z,
        //
        // Summary:
        //     Left curly bracket key '{'. Deprecated if "Use Physical Keys" is enabled in instead.
        LeftCurlyBracket = UnityEngine.KeyCode.LeftCurlyBracket,
        //
        // Summary:
        //     Pipe '|' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Pipe = UnityEngine.KeyCode.Pipe,
        //
        // Summary:
        //     Right curly bracket key '}'. Deprecated if "Use Physical Keys" is enabled in
        //     instead.
        RightCurlyBracket = UnityEngine.KeyCode.RightCurlyBracket,
        //
        // Summary:
        //     Tilde '~' key. Deprecated if "Use Physical Keys" is enabled in instead.
        Tilde = UnityEngine.KeyCode.Tilde,
        //
        // Summary:
        //     Numlock key.
        Numlock = UnityEngine.KeyCode.Numlock,
        //
        // Summary:
        //     Capslock key.
        CapsLock = UnityEngine.KeyCode.CapsLock,
        //
        // Summary:
        //     Scroll lock key.
        ScrollLock = UnityEngine.KeyCode.ScrollLock,
        //
        // Summary:
        //     Right shift key.
        RightShift = UnityEngine.KeyCode.RightShift,
        //
        // Summary:
        //     Left shift key.
        LeftShift = UnityEngine.KeyCode.LeftShift,
        //
        // Summary:
        //     Right Control key.
        RightControl = UnityEngine.KeyCode.RightControl,
        //
        // Summary:
        //     Left Control key.
        LeftControl = UnityEngine.KeyCode.LeftControl,
        //
        // Summary:
        //     Right Alt key.
        RightAlt = UnityEngine.KeyCode.RightAlt,
        //
        // Summary:
        //     Left Alt key.
        LeftAlt = UnityEngine.KeyCode.LeftAlt,
        //
        // Summary:
        //     Maps to left Windows key or left Command key if physical keys are enabled in
        //     Input Manager settings, otherwise maps to left Command key only.
        // LeftMeta = UnityEngine.KeyCode.LeftMeta,
        //
        // Summary:
        //     Left Command key.
        LeftCommand = UnityEngine.KeyCode.LeftCommand,
        //
        // Summary:
        //     Left Command key.
        // LeftApple = UnityEngine.KeyCode.LeftApple,
        //
        // Summary:
        //     Left Windows key. Deprecated if "Use Physical Keys" is enabled in instead.
        LeftWindows = UnityEngine.KeyCode.LeftWindows,
        //
        // Summary:
        //     Maps to right Windows key or right Command key if physical keys are enabled in
        //     Input Manager settings, otherwise maps to right Command key only.
        // RightMeta = UnityEngine.KeyCode.RightMeta,
        //
        // Summary:
        //     Right Command key.
        RightCommand = UnityEngine.KeyCode.RightCommand,
        //
        // Summary:
        //     Right Command key.
        // RightApple = UnityEngine.KeyCode.RightApple,
        //
        // Summary:
        //     Right Windows key. Deprecated if "Use Physical Keys" is enabled in instead.
        RightWindows = UnityEngine.KeyCode.RightWindows,
        //
        // Summary:
        //     Alt Gr key. Deprecated if "Use Physical Keys" is enabled in instead.
        AltGr = UnityEngine.KeyCode.AltGr,
        //
        // Summary:
        //     Help key. Deprecated if "Use Physical Keys" is enabled in, doesn't map to any
        //     physical key.
        Help = UnityEngine.KeyCode.Help,
        //
        // Summary:
        //     Print key.
        Print = UnityEngine.KeyCode.Print,
        //
        // Summary:
        //     Sys Req key. Deprecated if "Use Physical Keys" is enabled in, doesn't map to
        //     any physical key.
        SysReq = UnityEngine.KeyCode.SysReq,
        //
        // Summary:
        //     Break key. Deprecated if "Use Physical Keys" is enabled in, doesn't map to any
        //     physical key.
        Break = UnityEngine.KeyCode.Break,
        //
        // Summary:
        //     Menu key.
        Menu = UnityEngine.KeyCode.Menu,
    }
}
