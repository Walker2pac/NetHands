using System;

public class EnemyKillTimer
{
    private float _time = 15f;
    private float _currentTime = 0f;
    private Action _callback;
    private bool _isActive = false;
    public bool IsActive => _isActive;

    public EnemyKillTimer(float time, Action callback)
    {
        _time = time;
        _callback = callback;
    }

    public void Start()
    {
        _currentTime = 0f;
        _isActive = true;
    }

    public void Tick(float deltaTime)
    {
        if (!_isActive) return;

        _currentTime += deltaTime;

        if (_currentTime >= _time)
        {
            _callback?.Invoke();
            Stop();
        }
    }

    public void Stop()
    {
        _isActive = false;
    }
}
