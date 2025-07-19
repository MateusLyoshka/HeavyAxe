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

    void Start()
    {

    }

    public void DispenserDebris(Transform debrisPoint, Vector2 direction)
    {
        FakeHeight SingleDebri = Instantiate(singleDebri, debrisPoint.position, Quaternion.identity).GetComponent<FakeHeight>();
        SingleDebri.Initialize(direction * Random.Range(debriGroundVelocity.x, debriGroundVelocity.y), Random.Range(debriVerticalVelocity.x, debriVerticalVelocity.y));
    }

    void Update()
    {

    }
}
