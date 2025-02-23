using UnityEngine;

namespace SpaceShooterPro
{
    public interface ICollectable
    {
        void Collect(GameObject collector);
    }
}