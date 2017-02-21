using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LerpColor : MonoBehaviour {
    public Image Img;
    public Color defaultColor;
    private Color _tmp_color;
    private Color _tmp_color1;
    private bool _is_hide;
	// Use this for initialization
	void Start () 
    {
        defaultColor = Color.white;
        _tmp_color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
        Img.color = _tmp_color;
        _is_hide = true;
        //_tmp_color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!_is_hide)
        {
            Img.color = Color.Lerp(Img.color, _tmp_color, 0.04f);
            if (Img.color.a <= 0.1f)
            {
                Img.gameObject.SetActive(false);
                Img.color = defaultColor;
                Destroy(gameObject);
            }
        }
        else
        {
            Img.color = Color.Lerp(Img.color, defaultColor, 0.1f);
            if (Img.color.a >= 0.24f)
            {
                _is_hide = false;
            }
        }
	}
}
