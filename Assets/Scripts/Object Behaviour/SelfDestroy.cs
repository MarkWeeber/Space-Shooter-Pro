using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 3f;

    private void OnEnable()
    {
        Destroy(gameObject, _lifeTime);
    }
}
