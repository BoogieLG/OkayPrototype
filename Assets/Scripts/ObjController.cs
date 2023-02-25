using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class ObjController : MonoBehaviour
    {
        public float Speed;

        public Action OnResetGame;
        public Action<TargetController> OnHit;

        [SerializeField] Rigidbody _rb;

        Vector3 _startPosition;
        Vector3 _endPosition;
        Vector3 _direction;

        public void SetStartPosition(PointerEventData eventData)
        {
            OnResetGame?.Invoke();
            _rb.velocity = Vector3.zero;
            _startPosition = ConvertPos(eventData.position);
            _startPosition.z = 0.5f;
            transform.position = _startPosition;
        }

        public void SetEndPosition(PointerEventData eventData)
        {
            _endPosition = ConvertPos(eventData.position);
            _endPosition.z = 0.5f;
            StartMoving();
        }

        public void StartMoving()
        {
            _direction = _endPosition - _startPosition;
            _direction = _direction.normalized;
            _rb.velocity = _direction * Speed;

        }

        Vector3 ConvertPos(Vector3 pos)
        {
            pos.z = 10f;
            pos = Camera.main.ScreenToWorldPoint(pos);
            return pos;
        }

        private void OnCollisionEnter(Collision other)
        {
            var contact = other.GetContact(0);
            var angle = Vector3.SignedAngle(_direction, contact.normal, Vector3.forward);
            var newAngle = angle - 180;
            _direction = Quaternion.AngleAxis(newAngle, Vector3.forward) * contact.normal;
            _rb.velocity = (_direction) * Speed;
            OnHit?.Invoke(other.gameObject.GetComponent<TargetController>());

            if(Speed <= 0)
            {
                Debug.LogError("Set speed for ObjController 0+");
            }

        }
    }
}
