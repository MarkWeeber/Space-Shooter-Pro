using UnityEngine;

namespace Assets.Scripts._Refactoring
{
    public interface ICollectable
    {
        void Collect(GameObject collector);
    }
}