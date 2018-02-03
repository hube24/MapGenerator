using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    void Update()
    {
        //Up
        if (Input.GetKey(KeyCode.Space))
        {
            up_smooth += Time.unscaledDeltaTime * smooth_rate;
            up_smooth = Mathf.Clamp(up_smooth, 0f, 1f);
            down_smooth = 0f;
        }
        //Down
        else if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftControl))
        {
            down_smooth += Time.unscaledDeltaTime * smooth_rate;
            down_smooth = Mathf.Clamp(down_smooth, 0f, 1f);
            up_smooth = 0f;
        }
        else
        {
            down_smooth -= smooth_rate * Time.unscaledDeltaTime;
            down_smooth = Mathf.Clamp(down_smooth, 0f, 1f);
            up_smooth -= smooth_rate * Time.unscaledDeltaTime;
            up_smooth = Mathf.Clamp(up_smooth, 0f, 1f);
        }
        //Forward
        if (Input.GetKey(KeyCode.W))
        {
            forward_smooth += Time.unscaledDeltaTime * smooth_rate;
            forward_smooth = Mathf.Clamp(forward_smooth, 0f, 1f);
            backward_smooth = 0f;
        }
        //Backward
        else if (Input.GetKey(KeyCode.S))
        {
            backward_smooth += Time.unscaledDeltaTime * smooth_rate;
            backward_smooth = Mathf.Clamp(backward_smooth, 0f, 1f);
            forward_smooth = 0f;
        }
        else
        {
            forward_smooth -= smooth_rate * Time.unscaledDeltaTime;
            forward_smooth = Mathf.Clamp(forward_smooth, 0f, 1f);
            backward_smooth -= smooth_rate * Time.unscaledDeltaTime;
            backward_smooth = Mathf.Clamp(backward_smooth, 0f, 1f);
        }
        //Left
        if (Input.GetKey(KeyCode.A))
        {
            left_smooth += Time.unscaledDeltaTime * smooth_rate;
            left_smooth = Mathf.Clamp(left_smooth, 0f, 1f);
            right_smooth = 0f;
        }
        //Right
        else if (Input.GetKey(KeyCode.D))
        {
            right_smooth += Time.unscaledDeltaTime * smooth_rate;
            right_smooth = Mathf.Clamp(right_smooth, 0f, 1f);

            left_smooth = 0f;
        }
        else
        {
            left_smooth -= smooth_rate * Time.unscaledDeltaTime;
            left_smooth = Mathf.Clamp(left_smooth, 0f, 1f);
            right_smooth -= smooth_rate * Time.unscaledDeltaTime;
            right_smooth = Mathf.Clamp(right_smooth, 0f, 1f);
        }

        //Up/Down
        this.transform.Translate(Vector3.up * this.movement_rate * Time.unscaledDeltaTime * (up_smooth - down_smooth));
        //Forward/Backward
        this.transform.Translate(Vector3.forward  * this.movement_rate * Time.unscaledDeltaTime * (forward_smooth - backward_smooth));
        //Left/Right
        this.transform.Translate(Vector3.left  * this.movement_rate * Time.unscaledDeltaTime * (left_smooth - right_smooth));

        if (this.transform.parent != null)
        {
            this.transform.LookAt(this.transform.parent);
        }
    }

    //******************************************************************

    public float up_smooth = 0f;
    public float down_smooth = 0f;
    public float forward_smooth = 0f;
    public float backward_smooth = 0f;
    public float left_smooth = 0f;
    public float right_smooth = 0f;
    public float smooth_rate = 0.6f;
    public float movement_rate = 10f;
}
