using System;

namespace CoreLibrary
{
    public interface IConfigProvider
    {
        string GetString(string pathKey);
    }
}