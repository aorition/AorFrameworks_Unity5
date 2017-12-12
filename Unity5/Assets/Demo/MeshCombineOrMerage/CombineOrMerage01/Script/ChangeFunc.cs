using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChangeFunc : MonoBehaviour
{
    private int merageType = 0;
    public Transform notStatic;
    public Transform Combined;
    public Transform Meraged;

    public Text _btnLabel;

    private void Start()
    {
        _changeModel(1);

    }


    private void _changeModel(int newType)
    {

        switch (newType)
        {
            case 1:
                notStatic.gameObject.SetActive(false);
                Combined.gameObject.SetActive(true);
                Meraged.gameObject.SetActive(false);
                _btnLabel.text = "Combined";
                merageType = 1;
                break;
            case 2:
                notStatic.gameObject.SetActive(false);
                Combined.gameObject.SetActive(false);
                Meraged.gameObject.SetActive(true);
                _btnLabel.text = "Meraged";
                merageType = 2;
                break;
            default:
                notStatic.gameObject.SetActive(true);
                Combined.gameObject.SetActive(false);
                Meraged.gameObject.SetActive(false);
                _btnLabel.text = "Not Static";
                merageType = 0;
                break;
        }
        
    }

    public void ChangeModel()
    {
        int n = merageType + 1;
        if (n > 2)
        {
            n = 0;
        }
        _changeModel(n);
    }

}
