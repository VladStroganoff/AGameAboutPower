using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GAP_Server
{
    class Player
    {
        public int ID;
        public string name;
        public Vector3 position;
        public Quaternion rotation;


        private float movespeed = 5f / Constants.TicksPerSec;
        private bool[] inputs;

        public Player(int id, string username, Vector3 spawnpoint)
        {
            ID = id;
            name = username;
            position = spawnpoint;

            inputs = new bool[4];
        }


        public void Update()
        {
            Vector2 inputDirection = Vector2.Zero;
            if(inputs[0])
            {
                inputDirection.Y += 1;
            }
            if (inputs[1])
            {
                inputDirection.Y -= 1;
            }
            if (inputs[2])
            {
                inputDirection.X += 1;
            }
            if (inputs[3])
            {
                inputDirection.X -= 1;
            }

            Move(inputDirection);

        }

        private void Move(Vector2 inputDirection)
        {


            Vector3 forward = Vector3.Transform(new Vector3(0, 0, 1), rotation);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, new Vector3(0, 1, 0)));

            Vector3 moveDirection = right * inputDirection.X + forward * inputDirection.Y;
            position += moveDirection * movespeed;

            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
        }


        public void SetInput(bool[] _inputs, Quaternion _rotation)
        {
            inputs = _inputs;
            rotation = _rotation;
        }
    }
}
