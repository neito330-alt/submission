using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player_Controller : MonoBehaviour
{
    @DefaultInput _inputActions;
    Vector3 _initPos;
    public float Speed = 600.0f;
    void Awake(){
        //キーボードの入力を取得するためのインスタンスを作成
        _inputActions = new DefaultInput();
        _inputActions.Enable();
    }
    void Start(){
       // _initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var move = _inputActions.Player.Move.ReadValue<Vector2>();
        //前後左右の移動
        var forward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1,0,1)).normalized;
        var right = Camera.main.transform.right;
        
        var Player_Vector = forward * move.y + right * move.x;
        Player_Vector *= Speed * Time.deltaTime;

        var Player_Body = GetComponent<Rigidbody>();
        Player_Body.AddForce(Player_Vector);

        var velocity  = Player_Body.velocity;
        velocity.y = 0;
        if (velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
        }
    }
}
