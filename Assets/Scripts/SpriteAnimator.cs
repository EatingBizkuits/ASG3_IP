using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour
{
    public float speed;
    public int frameRate = 12;
    private Image _image;

    private Sprite[] _allSprites;
    private float _timePerFrame;
    private float _elapsedTime;
    private int _currentFrame;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        _image = GetComponent<Image>();
        enabled = false;
        LoadSpriteSheet();
    }

    // Update is called once per frame
    private void Update()
    {
        _elapsedTime += Time.deltaTime * speed;
        if (!(_elapsedTime >= _timePerFrame)) return;
        _elapsedTime = 0;
        ++_currentFrame;
        SetSprite();
        if (_currentFrame < _allSprites.Length) return;
        _currentFrame = 0;
    }

    private void LoadSpriteSheet()
    {
        _allSprites = Resources.LoadAll<Sprite>("grunge");
        if (_allSprites != null && _allSprites.Length > 0)
        {
            _timePerFrame = 1f / frameRate;
            PlayAnim();
        }
        else
        {
            Debug.Log("Error Loading Sprite");
        }
    }

    public void PlayAnim()
    {
        enabled = true;
    }

    private void SetSprite()
    {
        if (_currentFrame >= 0 && _currentFrame < _allSprites.Length) _image.sprite = _allSprites[_currentFrame];
    }
}
