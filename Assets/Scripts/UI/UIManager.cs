using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SpaceShooterPro
{
    public class UIManager : SingletonBehaviour<UIManager>
    {
        [SerializeField] private TMP_Text _scoreValueText;
        [SerializeField] private TMP_Text _ammoValueText;
        [SerializeField] private TMP_Text _announceText;
        [SerializeField] private float _announcementDuration = 3f;
        [SerializeField] private float _winAnnouncementDuration = 10f;
        [SerializeField] private Image _healthBarHorizontalFillImage;
        [SerializeField] private Color _fullHealthBarColor = Color.green;
        [SerializeField] private Color _emptyHealthBarColor = Color.red;
        [SerializeField] private Image[] _healthBarImages;
        [SerializeField] private Color _sprintBarCooldownColor = Color.yellow;
        [SerializeField] private Image _sprintBarFillImage;
        [SerializeField] private Image _sprintBarImage;
        [SerializeField] private float _cameraShakeIntensity = 0.05f;
        [SerializeField] private float _cameraShakeDuration = 0.4f;
        [SerializeField] private Transform _bossHealthPanel;
        [SerializeField] private Image _bossHealthBarHorizontalFillImage;
        [SerializeField] private Image[] _bossHealthBarImages;

        public UnityEvent OnGameOver;
        private IEnumerator _sprintBarCoolDownRoutine, _cameraShakeRoutine, _announcementRoutine;
        private Color _healthBarColor, _bossHealthBarColor;
        private Color _sprintBarFullColor;
        private float _healthBarFillRatio, _sprintBarFillRatio, _bossHealthBarFillRatio;
        private int _scoreValue;
        private Vector3 _cameraOriginalPosition;

        private void Start()
        {
            _scoreValueText.text = _scoreValue.ToString();
            _sprintBarFullColor = _sprintBarImage.color;
            _sprintBarCoolDownRoutine = SprintBarCooldownRoutine(0, 1);
            _cameraShakeRoutine = CamerShakeRoutine(0, 1);
            _announcementRoutine = DisplayAnnounceText("", 0);
            _cameraOriginalPosition = Camera.main.transform.position;
        }

        public void UpdateHealth(float startingHealth, float currentHealth)
        {
            if (_healthBarHorizontalFillImage != null)
            {
                _healthBarFillRatio = currentHealth / startingHealth;
                _healthBarHorizontalFillImage.fillAmount = _healthBarFillRatio;
                if (_healthBarImages != null)
                {
                    _healthBarColor = Color.Lerp(_emptyHealthBarColor, _fullHealthBarColor, _healthBarFillRatio);
                    foreach (var image in _healthBarImages)
                    {
                        image.color = _healthBarColor;
                    }
                }
            }
        }

        public void UpdateBossHealth(float startingHealth, float currentHealth)
        {
            if (_bossHealthBarHorizontalFillImage != null)
            {
                _bossHealthBarFillRatio = currentHealth / startingHealth;
                _bossHealthBarHorizontalFillImage.fillAmount = _bossHealthBarFillRatio;
                if (_bossHealthBarImages != null)
                {
                    _bossHealthBarColor = Color.Lerp(_emptyHealthBarColor, _fullHealthBarColor, _bossHealthBarFillRatio);
                    foreach (var image in _bossHealthBarImages)
                    {
                        image.color = _bossHealthBarColor;
                    }
                }
            }
        }

        public void SetSprintBarFillAmout(float fillAmount)
        {
            _sprintBarFillRatio = fillAmount;
            _sprintBarFillImage.fillAmount = _sprintBarFillRatio;
        }

        public void CooldownSprintBar(float timeInSeconds)
        {
            _sprintBarFillRatio = 0f;
            _sprintBarFillImage.fillAmount = _sprintBarFillRatio;
            _sprintBarImage.color = _sprintBarCooldownColor;
            StopCoroutine(_sprintBarCoolDownRoutine);
            _sprintBarCoolDownRoutine = SprintBarCooldownRoutine(timeInSeconds, 16);
            StartCoroutine(_sprintBarCoolDownRoutine);
        }

        public void AddScore(int score)
        {
            _scoreValue += score;
            _scoreValueText.text = _scoreValue.ToString();
        }

        public void SetAmmo(int ammoCount)
        {
            _ammoValueText.text = ammoCount.ToString();
        }

        public void ShakeCamera()
        {
            StopCoroutine(_cameraShakeRoutine);
            _cameraShakeRoutine = CamerShakeRoutine(_cameraShakeDuration, 9);
            StartCoroutine(_cameraShakeRoutine);
        }

        public void MakeAnnouncement(string text)
        {
            Announce(text, _announcementDuration);
        }

        private void Announce(string text, float duration)
        {
            StopCoroutine(_announcementRoutine);
            _announceText.text = "";
            _announcementRoutine = DisplayAnnounceText(text, duration);
            StartCoroutine(_announcementRoutine);
        }

        public void SetGameOver()
        {
            OnGameOver?.Invoke();
        }

        public void BossSpawned()
        {
            _bossHealthPanel.gameObject.SetActive(true);
        }

        public void BossDefeated()
        {
            _bossHealthPanel.gameObject.SetActive(false);
            Announce("CONGRATULATIONS, YOU DEFEATED THE BOSS!", _winAnnouncementDuration);

        }

        IEnumerator SprintBarCooldownRoutine(float timeInSeconds, int steps)
        {
            if (steps <= 0)
            {
                yield return null;
            }
            int currentStep = 0;
            while (currentStep < steps)
            {
                _sprintBarFillRatio = (float)currentStep / steps;
                _sprintBarFillImage.fillAmount = _sprintBarFillRatio;
                yield return new WaitForSeconds(timeInSeconds / steps);
                currentStep++;
            }
            _sprintBarFillImage.fillAmount = 1f;
            _sprintBarImage.color = _sprintBarFullColor;
        }

        IEnumerator CamerShakeRoutine(float timeInSeconds, int steps)
        {
            if (steps <= 0)
            {
                yield return null;
            }
            int currentStep = 0;
            while (currentStep < steps)
            {
                Camera.main.transform.position = _cameraOriginalPosition + Random.insideUnitSphere * _cameraShakeIntensity;
                yield return new WaitForSeconds(timeInSeconds / steps);
                currentStep++;
            }
            Camera.main.transform.position = _cameraOriginalPosition;
        }

        IEnumerator DisplayAnnounceText(string text, float duration)
        {
            _announceText.text = text;
            yield return new WaitForSeconds(duration);
            _announceText.text = "";
        }
    }
}