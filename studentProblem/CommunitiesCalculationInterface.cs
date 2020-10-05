using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace studentProblem
{
    interface CommunitiesCalculationInterface
    {
        RoomsStruct Calculate(List<Node> nodes, int nodesPerRoom);
        protected static List<Room> initRooms(int roomsAmount)
        {
            List<Room> rooms = new List<Room>();
            for (int i = 0; i < roomsAmount; i++)
            {
                Room roomStruct = new Room();
                roomStruct.nodes = new List<Node>();
                rooms.Add(roomStruct);
            }
            return rooms;
        }
    }
}
