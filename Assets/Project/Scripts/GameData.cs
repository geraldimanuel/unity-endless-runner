using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts
{
    public class GameData : MonoBehaviour
    {
        public static GameData Singleton;

        public Text scoreText;
        public Text highscoreText;

        public GameObject musicVolumeSlider;
        public GameObject sfxVolumeSlider;

        public float defaultMusicVolume = 0.25f;
        public float defaultSfxVolume = 0.5f;

        private readonly List<AudioSource> _music = new List<AudioSource>();
        private readonly List<AudioSource> _sfx = new List<AudioSource>();

        private int _score;
        private int _highScore;

        [NotNull]
        public AudioSource SoundPickup => _sfx[0];

        [NotNull]
        public AudioSource SoundJump => _sfx[1];

        [NotNull]
        public AudioSource SoundFootstep2 => _sfx[2];

        [NotNull]
        public AudioSource SoundFootstep1 => _sfx[3];

        [NotNull]
        public AudioSource SoundExplosion => _sfx[4];

        [NotNull]
        public AudioSource SoundDying => _sfx[5];

        [NotNull]
        public AudioSource SoundCastMagic => _sfx[6];

        [NotNull]
        public AudioSource SoundWhoosh => _sfx[7];

        public void AddScore(int value)
        {
            _score += value;
            PlayerPrefs.SetInt(PlayerPrefKeys.Score, _score);
            PlayerPrefs.SetInt(PlayerPrefKeys.LastScore, _score);
            UpdateScoreDisplay();
            UpdateHighscore();
        }

        private void UpdateHighscore()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefKeys.Highscore))
            {
                _highScore = 0;
                PlayerPrefs.SetInt(PlayerPrefKeys.Highscore, _highScore);
                return;
            }

            var oldHighscore = PlayerPrefs.GetInt(PlayerPrefKeys.Highscore);
            _highScore = Math.Max(oldHighscore, _score);
            PlayerPrefs.SetInt(PlayerPrefKeys.Highscore, _highScore);
        }

        public void ResetScore()
        {
            _score = 0;
            PlayerPrefs.SetInt(PlayerPrefKeys.Score, 0);
            UpdateScoreDisplay();
        }

        public void UpdateMusicVolume(float volume)
        {
            PlayerPrefs.SetFloat(PlayerPrefKeys.MusicVolume, volume);
            foreach (var source in _music)
            {
                source.volume = volume;
            }
        }

        public void UpdateSoundVolume(float volume)
        {
            PlayerPrefs.SetFloat(PlayerPrefKeys.SoundVolume, volume);
            foreach (var source in _sfx)
            {
                source.volume = volume;
            }
        }

        private void Awake()
        {
            var gd = GameObject.FindGameObjectsWithTag("GameData");
            if (gd.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            Singleton = this;

            PlayerPrefs.SetInt(PlayerPrefKeys.Score, 0);

            var audioSources = GameObject.FindWithTag("GameData").GetComponentsInChildren<AudioSource>();
            Debug.Assert(audioSources.Length > 1, "allAS.Length > 0");

            _music.Add(audioSources[0]);

            for (var i = 1; i < audioSources.Length; ++i)
            {
                _sfx.Add(audioSources[i]);
            }

            InitializeMusicVolume();
            InitializeSoundVolume();
        }

        private void InitializeMusicVolume() =>
            UpdateMusicVolume(PlayerPrefs.HasKey(PlayerPrefKeys.MusicVolume)
                ? PlayerPrefs.GetFloat(PlayerPrefKeys.MusicVolume)
                : defaultMusicVolume);

        private void InitializeSoundVolume() =>
            UpdateSoundVolume(PlayerPrefs.HasKey(PlayerPrefKeys.SoundVolume)
                ? PlayerPrefs.GetFloat(PlayerPrefKeys.SoundVolume)
                : defaultSfxVolume);

        private void UpdateScoreDisplay()
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {_score}";
            }

            if (highscoreText != null)
            {
                highscoreText.text = $"Highscore: {_highScore}";
            }
        }
    }
}
