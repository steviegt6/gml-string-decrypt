using MonoMod.Cil;

namespace GmlStringDecrypt.Readers
{
    public interface IILReader<out TData>
    {
        TData Read(ILCursor c);
    }
}