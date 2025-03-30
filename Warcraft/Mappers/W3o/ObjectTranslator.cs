using Warcraft.Models;

namespace Warcraft.Mappers.W3o;

public enum ObjectType
{
    Units,
    Items,
    Destructables,
    Doodads,
    Abilities,
    Buffs,
    Upgrades
}

public static class ObjectTranslator
{
    public static Wc3Data WarToJson(byte[] buffer, ObjectType type)
    {
        var result = new Wc3Data();
        var outBufferToJSON = new W3Buffer(buffer);

        var fileVersion = outBufferToJSON.ReadInt();

        void ReadModificationTable(bool isOriginalTable)
        {
            var numTableModifications = outBufferToJSON.ReadInt();

            for (var i = 0; i < numTableModifications; i++)
            {
                var originalId = outBufferToJSON.ReadChars(4);
                var customId = outBufferToJSON.ReadChars(4);

                var wc3Obj = new Wc3Object() { Code = customId, OriginalCode = originalId };

                if (isOriginalTable)
                {
                    wc3Obj.Code = originalId;
                }

                var sets = fileVersion >= 3 ? outBufferToJSON.ReadInt() : 1;

                for (var j = 0; j < sets; j++)
                {
                    if (fileVersion >= 3)
                    {
                        outBufferToJSON.ReadInt();
                    }
                    var modificationCount = outBufferToJSON.ReadInt();

                    for (var k = 0; k < modificationCount; k++)
                    {
                        var modification = new Warcraft3Field()
                        {
                            Id = outBufferToJSON.ReadChars(4),
                            Type = GetVarType(outBufferToJSON.ReadInt())
                        };

                        if (type == ObjectType.Doodads || type == ObjectType.Abilities || type == ObjectType.Upgrades)
                        {
                            modification.Level = outBufferToJSON.ReadInt();
                            modification.Column = outBufferToJSON.ReadInt();
                        }

                        if (modification.Type == "int")
                        {
                            modification.Value = outBufferToJSON.ReadInt();
                        }
                        else if (modification.Type == "real" || modification.Type == "unreal")
                        {
                            modification.Value = outBufferToJSON.ReadFloat();
                        }
                        else
                        {
                            modification.Value = outBufferToJSON.ReadString();
                        }

                        if (isOriginalTable)
                        {
                            outBufferToJSON.ReadInt();
                        }
                        else
                        {
                            outBufferToJSON.ReadChars(4);
                        }

                        wc3Obj.Fields.Add(modification);
                    }
                }

                if (isOriginalTable)
                {
                    result.Original.Add(wc3Obj);
                }
                else
                {
                    result.Custom.Add(wc3Obj);
                }
            }
        }

        ReadModificationTable(true);
        ReadModificationTable(false);

        return result;
    }

    private static string GetVarType(int type)
    {
        return type switch
        {
            0 => "int",
            1 => "real",
            2 => "unreal",
            _ => "string",
        };
    }
}
