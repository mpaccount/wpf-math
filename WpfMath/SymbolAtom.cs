using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Atom representing symbol (non-alphanumeric character).
internal class SymbolAtom : CharSymbol
{
    // Dictionary of definitions of all symbols, keyed by name.
    private static IDictionary<string, SymbolAtom> symbols;

    // Set of all valid symbol types.
    private static BitArray validSymbolTypes;

    static SymbolAtom()
    {
        var symbolParser = new TexSymbolParser();
        symbols = symbolParser.GetSymbols();

        validSymbolTypes = new BitArray(16);
        validSymbolTypes.Set((int)TexAtomType.Ordinary, true);
        validSymbolTypes.Set((int)TexAtomType.BigOperator, true);
        validSymbolTypes.Set((int)TexAtomType.BinaryOperator, true);
        validSymbolTypes.Set((int)TexAtomType.Relation, true);
        validSymbolTypes.Set((int)TexAtomType.Opening, true);
        validSymbolTypes.Set((int)TexAtomType.Closing, true);
        validSymbolTypes.Set((int)TexAtomType.Punctuation, true);
        validSymbolTypes.Set((int)TexAtomType.Accent, true);
    }

    public static SymbolAtom GetAtom(string name)
    {
        try
        {
            return symbols[name];
        }
        catch (KeyNotFoundException)
        {
            throw new SymbolNotFoundException(name);
        }
    }

    public SymbolAtom(SymbolAtom symbolAtom, TexAtomType type)
        : base()
    {
        if (!validSymbolTypes[(int)type])
            throw new ArgumentException("The specified type is not a valid symbol type.", "type");
        this.Type = type;
        this.Name = symbolAtom.Name;
        this.IsDelimeter = symbolAtom.IsDelimeter;
    }

    public SymbolAtom(string name, TexAtomType type, bool isDelimeter)
        : base()
    {
        this.Type = type;
        this.Name = name;
        this.IsDelimeter = isDelimeter;
    }

    public bool IsDelimeter
    {
        get;
        private set;
    }

    public string Name
    {
        get;
        private set;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        return new CharBox(environment, environment.TexFont.GetCharInfo(this.Name, environment.Style));
    }

    public override CharFont GetCharFont(ITeXFont texFont)
    {
        // Style is irrelevant here.
        return texFont.GetCharInfo(Name, TexStyle.Display).GetCharacterFont();
    }
}
