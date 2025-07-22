using UnityEngine;
using UnityEngine.UIElements;
using GD.MinMaxSlider;

public class DesbrisDispenser : MonoBehaviour
{
    public GameObject singleDebri;

    [MinMaxSlider(3, 5)]
    public Vector2 debriVerticalVelocity;
    [MinMaxSlider(3, 6)]
    public Vector2 debriGroundVelocity;

    [MinMaxSlider(2, 30)]
    public Vector2 debriSizeRange;

    [MinMaxSlider(3, 15)]
    public Vector2Int debrisAmount;

    public int debrisDispenseDegrees;
    private Vector2 debriDirection;

    public void DispenserDebris(Transform debrisPoint, Vector2 direction)
    {

        for (int i = 0; i < Random.Range(debrisAmount.x, debrisAmount.y); i++)
        {
            float randomizedDirectionAngle = Mathf.Atan2(direction.y, direction.x) + (Random.Range(-debrisDispenseDegrees / 2, debrisDispenseDegrees / 2) * Mathf.Deg2Rad);
            debriDirection.x = Mathf.Cos(randomizedDirectionAngle);
            debriDirection.y = Mathf.Sin(randomizedDirectionAngle);
            debriDirection.Normalize();
            FakeHeight instantiatedSingleDebri = Instantiate(singleDebri, debrisPoint.position, Quaternion.identity).GetComponent<FakeHeight>();
            instantiatedSingleDebri.transform.localScale = Vector3.one * Random.Range(debriSizeRange.x, debriSizeRange.y);
            instantiatedSingleDebri.Initialize(debriDirection * Random.Range(debriGroundVelocity.x, debriGroundVelocity.y), Random.Range(debriVerticalVelocity.x, debriVerticalVelocity.y));
        }
    }

    void Update()
    {

    }
}
