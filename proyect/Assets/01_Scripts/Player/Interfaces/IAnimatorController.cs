/// <summary>
/// Interface for animator controllers
/// Interface Segregation Principle (ISP)
/// </summary>
public interface IAnimatorController
{
    void UpdateMovement(float velX, float velY, bool isMoving);
    void SetTrigger(string triggerName);
    void SetBool(string parameterName, bool value);
    void SetFloat(string parameterName, float value);
    void SetInt(string parameterName, int value);
}
