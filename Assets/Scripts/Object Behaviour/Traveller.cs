using System;
using UnityEngine;

public class Traveller : MonoBehaviour
{
    [Serializable]
    public enum TravelDirection
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private float _rotationSpeed = 0f;
    [SerializeField] private Vector3 _bounds = new Vector3(5f, 10f, 0f);
    [SerializeField] private TravelDirection _travelDirection = TravelDirection.Up;

    private Vector3 _direction = Vector3.zero;

    private void Start()
    {
        SetUpVectors();
    }

    private void OnEnable()
    {
        Destroy(this.gameObject, _lifeTime);
    }

    private void Update()
    {
        if (enabled)
        {
            ManageTravelling(Time.deltaTime);
        }
        ManageOutOfBounds();
    }

    private void SetUpVectors()
    {
        _bounds = new Vector3(
        Mathf.Abs(_bounds.x),
        Mathf.Abs(_bounds.y),
        Mathf.Abs(_bounds.z)
    );
        switch (_travelDirection)
        {
            case TravelDirection.Up:
                _direction = Vector2.up;
                break;
            case TravelDirection.Down:
                _direction = Vector2.down;
                break;
            case TravelDirection.Left:
                _direction = Vector2.left;
                break;
            case TravelDirection.Right:
                _direction = Vector2.right;
                break;
            default:
                _direction = Vector2.up;
                break;
        }
    }

    private void ManageTravelling(float deltaTime)
    {
        transform.Translate(_direction * _speed * deltaTime);
        if (_rotationSpeed > 0f)
        {
            transform.Rotate(Vector3.forward * _rotationSpeed * deltaTime);
        }
    }

    private void ManageOutOfBounds()
    {
        if (transform.position.x < -_bounds.x || transform.position.x > _bounds.x)
        {
            Destroy(this.gameObject);
        }
        if (transform.position.y < -_bounds.y || transform.position.y > _bounds.y)
        {
            Destroy(this.gameObject);
        }
    }
}