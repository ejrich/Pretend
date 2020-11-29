using System;
using System.Collections.Generic;

namespace Pretend
{
    public enum KeyCode
    {
        // Numbers
        Zero = 48,
        One = 49,
        Two = 50,
        Three = 51,
        Four = 52,
        Five = 53,
        Six = 54,
        Seven = 55,
        Eight = 56,
        Nine = 57,

        // Letters
        A = 97,
        B = 98,
        C = 99,
        D = 100,
        E = 101,
        F = 102,
        G = 103,
        H = 104,
        I = 105,
        J = 106,
        K = 107,
        L = 108,
        M = 109,
        N = 110,
        O = 111,
        P = 112,
        Q = 113,
        R = 114,
        S = 115,
        T = 116,
        U = 117,
        V = 118,
        W = 119,
        X = 120,
        Y = 121,
        Z = 122,

        // Function Row
        Escape = 27,
        F1 = 1073741882,
        F2 = 1073741883,
        F3 = 1073741884,
        F4= 1073741885,
        F5 = 1073741886,
        F6 = 1073741887,
        F7 = 1073741888,
        F8 = 1073741889,
        F9 = 1073741890,
        F10 = 1073741891,
        F11 = 1073741892,
        F12 = 1073741893,

        // Modifier Keys
        LeftControl = 1073742048,
        LeftShift = 1073742049,
        LeftAlt = 1073742050,
        CapsLock = 1073741881,
        RightControl = 1073742052,
        RightShift = 1073742053,
        RightAlt = 1073742054,

        // Characters
        Comma = 44,
        Period = 46,
        ForwardSlash = 47,
        Semicolon = 59,
        Apostrophe = 39,
        OpenBracket = 91,
        CloseBracket = 93,
        BackSlash = 92,
        Minus = 45,
        Equals = 61,
        Tick = 96,

        // Shift Characters
        Tilde = 126,
        Exclamation = 33,
        At = 64,
        Pound = 35,
        Dollar = 36,
        Percent = 37,
        Caret = 94,
        Ampersand = 38,
        Multiply = 42,
        OpenParenthesis = 40,
        CloseParenthesis = 41,
        Underscore = 95,
        Plus = 43,
        OpenBrace = 123,
        CloseBrace = 125,
        Pipe = 124,
        Colon = 58,
        Quotation = 34,
        LessThan = 60,
        GreaterThan = 62,
        Question = 63,

        // Whitespace
        Space = 32,
        Enter = 13,
        Tab = 9,
        Backspace = 8,

        // Directions
        Up = 1073741906,
        Down = 1073741905,
        Left = 1073741904,
        Right = 1073741903,
    }

    [Flags]
    public enum KeyMod
    {
        None = 0x0000,
        LeftShift = 0x0001,
        RightShift = 0x0002,
        LeftControl = 0x0040,
        RightControl = 0x0080,
        LeftAlt = 0x0100,
        RightAlt = 0x0200,
        NumLock = 0x1000,
        CapsLock = 0x2000,

        Shift = (LeftShift | RightShift),
        Control = (LeftControl | RightControl),
        Alt = (LeftAlt | RightAlt)
    }

    public static class KeyCodeExtensions
    {
        private static readonly IDictionary<KeyCode, KeyCode> ShiftCodes = new Dictionary<KeyCode, KeyCode>
        {
            { KeyCode.Tick, KeyCode.Tilde },
            { KeyCode.One, KeyCode.Exclamation },
            { KeyCode.Two, KeyCode.At },
            { KeyCode.Three, KeyCode.Pound },
            { KeyCode.Four, KeyCode.Dollar },
            { KeyCode.Five, KeyCode.Percent },
            { KeyCode.Six, KeyCode.Caret },
            { KeyCode.Seven, KeyCode.Ampersand },
            { KeyCode.Eight, KeyCode.Multiply },
            { KeyCode.Nine, KeyCode.OpenParenthesis },
            { KeyCode.Zero, KeyCode.CloseParenthesis },
            { KeyCode.Minus, KeyCode.Underscore },
            { KeyCode.Equals, KeyCode.Plus },
            { KeyCode.OpenBracket, KeyCode.OpenBrace },
            { KeyCode.CloseBracket, KeyCode.CloseBrace },
            { KeyCode.BackSlash, KeyCode.Pipe },
            { KeyCode.Semicolon, KeyCode.Colon },
            { KeyCode.Apostrophe, KeyCode.Quotation },
            { KeyCode.Comma, KeyCode.LessThan },
            { KeyCode.Period, KeyCode.GreaterThan },
            { KeyCode.ForwardSlash, KeyCode.Question }
        };

        private static readonly ISet<KeyCode> ReservedCharacters = new HashSet<KeyCode>
        {
            KeyCode.Backspace, KeyCode.Tab, KeyCode.Enter
        };
 
        public static KeyCode GetKeyCodeWithMod(this KeyCode keyCode, KeyMod mod)
        {
            // Handle letters
            if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
                return keyCode;

            if ((mod & KeyMod.Shift) != 0 && ShiftCodes.ContainsKey(keyCode))
            {
                return ShiftCodes[keyCode];
            }
            return keyCode;
        }

        public static char GetChar(this KeyCode keyCode, KeyMod mod)
        {
            var characterCode = GetKeyCodeWithMod(keyCode, mod);

            // Handle letters
            if (characterCode >= KeyCode.A && characterCode <= KeyCode.Z)
            {
                var caps = mod.HasFlag(KeyMod.CapsLock);
                var shift = (mod & KeyMod.Shift) != 0;

                return (char)(characterCode - (caps ^ shift ? 0x20 : 0));
            }

            // Handle special characters
            if (characterCode >= KeyCode.Space && characterCode <= KeyCode.Tilde)
                return (char)characterCode;

            // Handle reserved characters
            if (ReservedCharacters.Contains(characterCode))
                return (char)characterCode;

            return '\0';
        }
    }
}
