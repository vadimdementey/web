using System;

namespace CoreLibrary
{
    public interface ITypeProvider
    {
        Type GetType(string typeName);
        void SetType(string typeName,Type typeInstance);
    }
}