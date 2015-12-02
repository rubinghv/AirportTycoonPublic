using UnityEngine;
using System.Collections;

public class SteeringCalculate : EntityNoMono {

    public bool seek,
                flee,
                arrive,
                separation,
                pursuit;

    //public enum behaviors { seek = false, flee = false, arrive = false, separation = false, pursuit = false; };

    public virtual Vector2 CalculateSteering(MovingEntity vehicle) { Debug.Log("ERROR!");  return new Vector2(0, 0); }

    

}
