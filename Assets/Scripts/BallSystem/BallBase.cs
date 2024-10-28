using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public abstract class BallBase : MonoBehaviour, IBall
{
    [SerializeField] int _startBallNumber = 2;
    protected BallData _ballData;
    protected TextMeshPro _tmp;
    protected Renderer _ballRenderer;

    private void Awake()
    {
        _ballData = new BallData(_startBallNumber);
        _tmp = GetComponentInChildren<TextMeshPro>();
        _tmp.text = _ballData.Number.ToString();
        _ballRenderer = GetComponent<Renderer>();
        ChangeColor();
    }

    public virtual void IncreaseNumber()
    {
        _ballData.Number *= 2;
        _tmp.text = _ballData.Number.ToString();
        ChangeColor();
    }

    public void SetNumber(int number)
    {
        _ballData.Number = number;
        _tmp.text = _ballData.Number.ToString();
        ChangeColor();
    }

    public void ChangeColor()
    {
        string materialPath = $"Materials/BallMaterials/{(_ballData.Number > 8196 ? "Any" : _ballData.Number.ToString())}";
        Material material = Resources.Load<Material>(materialPath);
        _ballRenderer.material = material;
    }

    public int ReturnBallNumber()
    {
        return _ballData.Number;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<ScoreBall>() != null)
        {
            if (collision.gameObject.GetComponent<ScoreBall>().ReturnBallNumber() == _ballData.Number)
            {
                Destroy(collision.gameObject);
                IncreaseNumber();
            }
        }
    }
}
