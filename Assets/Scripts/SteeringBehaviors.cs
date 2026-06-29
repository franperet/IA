using UnityEngine;

public static class SteeringBehaviors
{
    // Seek: va directo hacia el objetivo a velocidad maxima
    public static Vector3 Seek(Vector3 position, Vector3 target, float speed)
    {
        return (target - position).normalized * speed;
    }

    // Flee: huye en la direccion opuesta al objetivo
    public static Vector3 Flee(Vector3 position, Vector3 threat, float speed)
    {
        return (position - threat).normalized * speed;
    }

    // Arrive: como Seek pero frena al acercarse (evita overshooting)
    public static Vector3 Arrive(Vector3 position, Vector3 target, float speed, float slowRadius = 2f, float stopRadius = 0.15f)
    {
        Vector3 dir = target - position;
        float dist = dir.magnitude;
        if (dist < stopRadius) return Vector3.zero;
        float rampedSpeed = speed * (dist / slowRadius);
        return dir.normalized * Mathf.Min(rampedSpeed, speed);
    }

    // Wander: movimiento aleatorio natural con angulo que cambia suavemente
    public static Vector3 Wander(ref float wanderAngle, float speed, float changeRate = 45f)
    {
        wanderAngle += Random.Range(-changeRate, changeRate) * Time.deltaTime;
        float rad = wanderAngle * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
        return dir * speed;
    }

    // Pursue: predice la posicion futura del objetivo y busca interceptarlo
    public static Vector3 Pursue(Vector3 position, Vector3 targetPos, Vector3 targetVelocity, float speed)
    {
        float dist = Vector3.Distance(position, targetPos);
        float predictionTime = speed > 0 ? dist / speed : 0f;
        Vector3 futurePos = targetPos + targetVelocity * predictionTime;
        return Seek(position, futurePos, speed);
    }

    // Evade: predice la posicion futura de la amenaza y huye de ella
    public static Vector3 Evade(Vector3 position, Vector3 threatPos, Vector3 threatVelocity, float speed)
    {
        float dist = Vector3.Distance(position, threatPos);
        float predictionTime = speed > 0 ? dist / speed : 0f;
        Vector3 futurePos = threatPos + threatVelocity * predictionTime;
        return Flee(position, futurePos, speed);
    }
}
