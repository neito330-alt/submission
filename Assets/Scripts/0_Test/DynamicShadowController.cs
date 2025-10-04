using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Light))]
public class DynamicShadowController : MonoBehaviour
{
    [Header("シャドウ設定")]
    [SerializeField, Tooltip("シャドウを有効にするカメラからの最大距離")]
    private float _maxShadowDistance = 10f;

    [SerializeField]
    private float _minShadowDistance = 6f;


    [SerializeField]
    private float _maxLightDistance = 20f;

    [SerializeField]
    private float _minLightDistance = 6f;



    [SerializeField, Tooltip("更新間隔（フレーム数）")]
    private int _updateInterval = 3;

    private Light _light;
    private HDAdditionalLightData _hdLightData;
    private int _frameCount;

    [SerializeField]
    private float _nextShadowFactor;
    [SerializeField]
    private float _currentShadowFactor;

    [SerializeField]
    private float _nextLightFactor;
    [SerializeField]
    private float _currentLightFactor;

    [SerializeField]
    private float _shadowDalta;
    [SerializeField]
    private float _lightDalta;

    private bool _isShadowMove;
    private bool _isLightMove;

    [SerializeField]
    private float _defaultRange;

    [SerializeField]
    private float _defaultStrength;

    // 初期化
    void Awake()
    {
        _light = GetComponent<Light>();
        _hdLightData = GetComponent<HDAdditionalLightData>();

        if (_light == null || _hdLightData == null)
        {
            Debug.LogError("LightまたはHDAdditionalLightDataコンポーネントが見つかりません", this);
            enabled = false;
            return;
        }
        _defaultRange = _light.range;
        if (_hdLightData != null)_defaultStrength = _light.shadowStrength;

        // 初期状態でシャドウを無効化
        _light.shadows = LightShadows.None;
    }

    // 間隔を空けて更新（パフォーマンス最適化）
    void Update()
    {
        _frameCount++;
        if (_frameCount % _updateInterval != 0) return;

        _frameCount = 0;
        UpdateShadowState();

        if (_isShadowMove)
        {
            _currentShadowFactor += _shadowDalta * Time.deltaTime * 2;
            if (_shadowDalta > 0)
            {
                if (_currentShadowFactor > _nextShadowFactor)
                {
                    _currentShadowFactor = _nextShadowFactor;
                    _isShadowMove = false;
                }
            }
            else
            {
                if (_currentShadowFactor < _nextShadowFactor)
                {
                    _currentShadowFactor = _nextShadowFactor;
                    _isShadowMove = false;
                }
            }

            _light.shadows = _currentShadowFactor < 0.95f ? LightShadows.Soft : LightShadows.None;
            _light.shadowStrength = 1 - (_currentShadowFactor * _currentShadowFactor);
            if (_hdLightData != null)
            {
                _hdLightData.shadowDimmer = 1 - (_currentShadowFactor * _currentShadowFactor);
                _hdLightData.affectsVolumetric = _currentShadowFactor < 0.95f;
            }

        }

        if (_isLightMove)
        {
            _currentLightFactor += _lightDalta * Time.deltaTime*2;
            if (_lightDalta > 0)
            {
                if (_currentLightFactor > _nextLightFactor)
                {
                    _currentLightFactor = _nextLightFactor;
                    _isLightMove = false;
                }
            }
            else
            {
                if (_currentLightFactor < _nextLightFactor)
                {
                    _currentLightFactor = _nextLightFactor;
                    _isLightMove = false;
                }
            }
            _light.range = _defaultRange * (1 - (_currentLightFactor * _currentLightFactor));
        }
    }


    // シャドウ状態を更新
    private void UpdateShadowState()
    {
        if (Camera.main == null) return;

        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        bool shouldCastShadow = distance <= _maxShadowDistance;
        bool shouldCastLight = distance <= _maxLightDistance;

        // 状態が変化した時のみ更新
        if (_light.shadows != (shouldCastShadow ? LightShadows.Soft : LightShadows.None))
        {
            // _light.shadows = shouldCastShadow ? LightShadows.Soft : LightShadows.None;
            // // HDRP固有の設定
            // if (_hdLightData != null)
            // {
            //     _hdLightData.affectsVolumetric = shouldCastShadow;
            //     _hdLightData.volumetricShadowDimmer = shouldCastShadow ? 1.0f : 0.0f;
            // }
        }

        if (shouldCastShadow)
        {
            if (distance > _minShadowDistance)
            {
                float oldFactor = _nextShadowFactor;
                _nextShadowFactor = (distance - _minShadowDistance) / (_maxShadowDistance - _minShadowDistance);
                if (oldFactor != _nextShadowFactor) _shadowDalta = _nextShadowFactor - _currentShadowFactor;
                _isShadowMove = true;
            }
            else
            {
                if (_currentShadowFactor == 0f)
                {
                    _isShadowMove = false;
                }
                else
                {
                    _nextShadowFactor = 0;
                    _isShadowMove = true;
                }
            }
        }
        else
        {
            if (_currentShadowFactor > 1)
            {
                _nextShadowFactor = 1;
                _isShadowMove = true;
            }
        }


        if (shouldCastLight)
        {
            if (distance > _minLightDistance)
            {
                float oldFactor = _nextLightFactor;
                _nextLightFactor = (distance - _minLightDistance) / (_maxLightDistance - _minLightDistance);
                if(_nextLightFactor != oldFactor)_lightDalta = _nextLightFactor - _currentLightFactor;
                _isLightMove = true;
            }
            else
            {
                if (_currentLightFactor == 0f)
                {
                    _isLightMove = false;
                }
                else
                {
                    _nextLightFactor = 0;
                    _isLightMove = true;
                }
            }
        }
        else
        {
            if (_currentLightFactor > 1)
            {
                _nextLightFactor = 1;
                _isLightMove = true;
            }
        }


    }

    // デバッグ用ギズモ表示
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _maxShadowDistance);
    }
}
