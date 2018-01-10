using UnityEngine;

namespace Assets.ItemCombo
{
    /// <summary>
    /// 判定を行う
    /// </summary>
    public interface IResolver
    {
        bool Resolve(params string[] origin);
    }
}
