using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace studentProblem
{
    class NormalizeCommunities : CommunitiesCalculationInterface
    {
        public RoomsStruct Calculate(List<Node> nodes, int nodesPerRoom)
        {
            Random random = new Random();
            List<Node> unassignedNodes = new List<Node>(nodes);
            List<Node> nodesThatWereAdded = new List<Node>();
            int roomsAmount = (int) Math.Ceiling(nodes.Count / (decimal) nodesPerRoom);
            //initializes the rooms
            List<Room> rooms = CommunitiesCalculationInterface.initRooms(roomsAmount);
            List<Room> fullRooms = new List<Room>();
            bool hadMutualConnection = true;
            int iterationCounter = 0;
            while (unassignedNodes.Count != 0 && iterationCounter < nodesPerRoom * 2)
            {
                iterationCounter++;
                //sort the rooms from shortest in length to longest
                rooms.Sort(new Comparison<Room>((x, y) =>
                {
                    return x.nodes.Count > y.nodes.Count ? 1 : x.nodes.Count < y.nodes.Count ? -1 : 0;
                }));
                foreach (Room room in rooms)
                {
                    // if the room is marked as problematic, continue
                    if (room.isProblematic) continue;
                    //the room hit the maximum nodes it should contain
                    if (room.nodes.Count == nodesPerRoom)
                    {
                        fullRooms.Add(room);
                        continue;

                    }
                    // the room is empty
                    else if (room.nodes.Count == 0)
                    {
                        // we know in the last run we found a mutual connection between nodes, in that case, perhaps we can find another.
                        if (hadMutualConnection)
                        {
                            // look for a mutual connection
                            Node[] connectedNodes = findMutualConnection(unassignedNodes);
                            // a mutualConnection is found, move the nodes to the room
                            if (connectedNodes != null)
                            {
                                moveNodeFromListToRoom(connectedNodes[0], ref unassignedNodes, room);
                                moveNodeFromListToRoom(connectedNodes[1], ref unassignedNodes, room);

                            }
                            else
                            {
                                hadMutualConnection = false;
                                // move a random unassigned node to the room
                                moveNodeFromListToRoom(unassignedNodes[random.Next(unassignedNodes.Count)], ref unassignedNodes, room);
                            }
                        }
                        // we couldn't find a mutual connection earlier, so we shouldn't waste time looking for one
                        else
                        {
                            // move a random unassigned node to the room
                            moveNodeFromListToRoom(unassignedNodes[random.Next(unassignedNodes.Count)], ref unassignedNodes, room);
                        }
                    }
                    // the room contains one or more nodes, but isn't full
                    else
                    {
                        if (hadMutualConnection)
                        {
                            // the room has more than 1 empty slot
                            if (room.nodes.Count < nodesPerRoom - 1)
                            {
                                // look for a mutual connection
                                Node[] connectedNodes = findMutualConnection(unassignedNodes);
                                // a mutualConnection is found, move the nodes to the room
                                if (connectedNodes != null)
                                {
                                    moveNodeFromListToRoom(connectedNodes[0], ref unassignedNodes, room);
                                    moveNodeFromListToRoom(connectedNodes[1], ref unassignedNodes, room);
                                    //  we closed the room with a mutual connection, therefore we keep that in mind, so we won't manipulate it's last member
                                    if (room.nodes.Count == nodesPerRoom) room.closedWithMutualConnection = true;
                                    continue;
                                }
                                else
                                {
                                    hadMutualConnection = false;
                                }
                            }
                        }
                        // there is no mutual connection or the room has only 1 empty slot, try to add an unassignedNode to the room
                        bool addedNode = false;
                        foreach (Node node in unassignedNodes)
                        {
                            if (isRoomGoodForNode(node, room))
                            {
                                moveNodeFromListToRoom(node, ref unassignedNodes, room);
                                addedNode = true;
                                break;
                            }
                        }
                        if (!addedNode)
                        {
                            room.isProblematic = true;
                        }
                    }
                }

                // we didn't remove them earlier, so we remove them now
                foreach (Room fullRoom in fullRooms)
                {
                    rooms.Remove(fullRoom);
                }
            }
            // if some nodes are left unassigned, they are problematic and we should check if they can change a spot with another node that exists inside a room. the node that the problematic node is going to swtich places with should be the last node of the room, and only if the room wasn't closed with a mutual connection.
            if (unassignedNodes.Count != 0)
            {
                Console.WriteLine($"starting problematic nodes iteration with: {unassignedNodes.Count} problematic nodes");
                foreach (Node problematicNode in unassignedNodes)
                {
                    // because we already tried adding the nodes to each room, we know have to check only in the fullrooms
                    List<Room> roomsWithInterestingNodes = findRoomsWithConnection(problematicNode, fullRooms, nodesPerRoom);
                    foreach (Room room in roomsWithInterestingNodes)
                    {
                        // if the current room was closed with a mutual connection, we shouldn't move it's last node.
                        if (room.closedWithMutualConnection) continue;
                        Node lastNodeOfRoom = room.nodes.Last();
                        // we are now checking if we can find a room to add the lastNodeOfRoom to, we are iterating over the rooms array, these rooms are not full.
                        List<Room> lastNodeRooms = findRoomsWithConnection(lastNodeOfRoom, rooms, nodesPerRoom);
                        // we found a room to add the lastNodeOfRoom to!
                        if (lastNodeRooms.Count != 0)
                        {
                            // add the nodes to their new rooms
                            moveNodeFromRoomToRoom(lastNodeOfRoom, room, lastNodeRooms[0]);
                            room.nodes.Add(problematicNode);
                            nodesThatWereAdded.Add(problematicNode);
                            break;
                        }
                    }
                }
                foreach (Node node in nodesThatWereAdded)
                {
                    // remove all the nodes that were added now, after the iteration is complete.
                    unassignedNodes.Remove(node);
                }
            }
            RoomsStruct roomsStruct = new RoomsStruct();
            roomsStruct.unassignedNodes = unassignedNodes.ToArray();
            roomsStruct.rooms = rooms.Concat(fullRooms).ToArray();
            return roomsStruct;
        }

        private Node[] findMutualConnection(List<Node> nodes)
        {
            foreach (Node currentUnassignedNode in nodes)
            {
                List<Node> currentInterestingNodes = new List<Node>(currentUnassignedNode.InterestedIn);
                // for each node that the currentUnassignedNode is interested in
                foreach (Node interestingNode in currentInterestingNodes)
                {
                    // the interestingNode in interested in the currentUnassignedNode, therefore we have a mutual connection
                    if (nodes.Contains(interestingNode) && interestingNode.InterestedIn.Contains(currentUnassignedNode))
                    {
                        Node[] connectedNodes = { currentUnassignedNode, interestingNode };
                        return connectedNodes;
                    }
                }
            }
            // we couldn't find a mutual connection
            return null;
        }

        // this function finds rooms that contains nodes that the given node is interested in, BUT the last node, because returned rooms will be used to change a spot with the last node.
        private List<Room> findRoomsWithConnection(Node node, List<Room> rooms, int nodesPerRoom)
        {
            List<Room> roomsWithConnections = new List<Room>();
            foreach (Room room in rooms)
            {
                // for each node that the node is interested in
                foreach (Node interestingNode in node.InterestedIn)
                {
                    // the room contains the interestingNode
                    if (room.nodes.Contains(interestingNode))
                    {
                        // the room is at it's maximum capacity, therefore we should make sure that the interesting node isn't the last one
                        if (room.nodes.Count == nodesPerRoom)
                        {
                            // the interesting node isn't the last node of the room, we check that because we will use the room to change a slot with the last node
                            if (interestingNode != room.nodes.Last())
                            {
                                // we haven't added the room yet
                                if (!roomsWithConnections.Contains(room))
                                {
                                    roomsWithConnections.Add(room);
                                }
                            }
                        }
                        else
                        {
                            // we haven't added the room yet
                            if (!roomsWithConnections.Contains(room))
                            {
                                roomsWithConnections.Add(room);
                            }
                        }

                    }
                }
            }
            return roomsWithConnections;
        }

        private bool isRoomGoodForNode(Node node, Room room)
        {
            foreach (Node interestingNode in node.InterestedIn)
            {
                if (room.nodes.Contains(interestingNode)) return true;
            }
            return false;
        }

        private void moveNodeFromListToRoom(Node node, ref List<Node> nodes ,Room room)
        {
            room.nodes.Add(node);
            nodes.Remove(node);
        }

        private void moveNodeFromRoomToRoom(Node node, Room fromRoom, Room ToRoom)
        {
            ToRoom.nodes.Add(node);
            fromRoom.nodes.Remove(node);
        }
    }
}
