using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float destoryTime;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Color damageColor;
    [SerializeField] private Vector3 randomOffset;

    private TextMeshPro damageText;

    private void Awake() 
    {
        damageText = GetComponent<TextMeshPro>();
        transform.localPosition += offset;
        transform.localPosition += new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(-randomOffset.y,randomOffset.y),
            Random.Range(-randomOffset.z,randomOffset.z));
        Destroy(gameObject, destoryTime);
    }

    public void Initalise(int damageValue)
    {
        damageText.text = damageValue.ToString();
    }
}
