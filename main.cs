using System;
using System.Collections.Generic;

public class Tyle {
    public int Left,Right;
}
public enum Room_Type {
    Unassigned,
    Start,
    Exit,
    Maze
}
public class Room{
    public Room_Type Type;
    public int Width, Height;
    public int X,Y;
}

public class Program {

    public const int MAP_MAX = 40;
    public const int MAP_MIN = 20;
    public const int ROOM_MAX = 10;
    public const int ROOM_MIN = 4;

    public static void Main(string[] args) {
        // Instantiate random number generator using system-supplied value as seed.
        var rand = new Random();
        int width = rand.Next(MAP_MIN,MAP_MAX+1);
        int height = rand.Next(MAP_MIN,MAP_MAX+1);

        // generate a standard 2d map arr
        int[,] gen = new int[height,width];
        // create border walls
        for(int i = 0; i < height; i++) {
            for(int j = 0; j < width; j++) {
                if(i == 0 || i == height-1) gen[i,j] = 1;
                else if(j == 0 || j == width-1) gen[i,j] = 1;
            }
        }
        // generate rooms
        List<Room> rooms = new List<Room>();
        Queue<int> addresses = new Queue<int>();
        addresses.Enqueue(1*width+1); //add (1,1) to queue
        while(addresses.Count > 0) {
            //determine if width or height is longer
            int Right = 0,down = 0;
            int pos = addresses.Peek();
            while(gen[pos/width, pos%width] != 1) {
                Right++;
                pos++;
            }
            pos = addresses.Peek();
            while(gen[pos/width, pos%width] != 1) {
                down++;
                pos+=width;
            }
            //if the rectangle is small enough to be considered a room;
            if( (Right > down ? Right : down) < ROOM_MAX) {
                rooms.Add(new Room() {Type = Room_Type.Unassigned ,Width = Right, Height = down, X = addresses.Peek()%width, Y = addresses.Peek()/width});
                addresses.Dequeue();
            }
            else { // the retancle needs to be divided
                // where wall will created after being added to pos
                int div = rand.Next(ROOM_MIN, ROOM_MAX);
                pos = addresses.Peek();
                if(Right > down) { //cut vertically
                    // if rectangle being created is big enough add to queue
                    if(width-(pos%width+div+1) > ROOM_MIN ) addresses.Enqueue(pos+div+1);
                    while(gen[pos/width, pos%width+div] != 1) {
                        gen[pos/width, pos%width+div] = 1;
                        pos+=width;
                    }
                } else { //cut horizontally
                    // if rectangle being created is big enough add to queue
                    if(height-(pos/width+div+1) > ROOM_MIN ) addresses.Enqueue(pos+(div+1)*width);
                    while(gen[pos/width+div, pos%width] != 1) {
                        gen[pos/width+div, pos%width] = 1;
                        pos++;
                    }
                }
            }
        }
        // pick rooms
        // get a random start room
        int start = rand.Next(rooms.Count);
        rooms[start].Type = Room_Type.Start;
        List<int> psbl = new List<int>();
        for(int i=0; i<rooms.Count; i++) {
            if(Math.Abs(rooms[i].X-rooms[start].X)+Math.Abs(rooms[i].Y-rooms[start].Y) > (width+height)/3) {
                psbl.Add(i);
            }
        }
        int end = psbl[rand.Next(psbl.Count)];
        rooms[end].Type = Room_Type.Exit;
        // // show room numbers
        // for(int i=0; i<rooms.Count; i++) {
        //     gen[rooms[i].Y,rooms[i].X] = i;
        // }
        Console.Write("possible rooms: ");
        foreach(int i in psbl) 
            Console.Write(i+" ");
        Console.WriteLine("\nStart: "+start+"\nExit: "+end);
        // initialize the rooms that are unnassigned as maze rooms
        foreach(Room r in rooms) {
            if(r.Type == Room_Type.Unassigned) 
                r.Type = Room_Type.Maze;
        }
        // initialize doors
        int room_num = 0;
        foreach (Room r in rooms) {
            // Check if the room is a maze type
            if (r.Type != Room_Type.Maze) {
                // Check if room is start or end
                if (r.Type != Room_Type.Start) {
                    gen[rooms[room_num].Y,rooms[room_num].X] = 3;
                } else if (r.Type != Room_Type.Exit) {
                    gen[rooms[room_num].Y,rooms[room_num].X] = 4;
                }
                // Array to check each wall for bounds
                bool[] walls = new bool[4];
                // Check if a wall goes out of bounds
                if (rooms[room_num].Y - 2 <= 0) {
                    walls[0] = true;
                } 
                if (rooms[room_num].Y + r.Height + 1 >= height) {
                    walls[1] = true;
                }
                if (rooms[room_num].X - 2 <= 0) {
                    walls[2] = true;
                }
                if (rooms[room_num].X + r.Width + 1 >= width) {
                    walls[3] = true;
                }

                //List of possible doors
                var door_list = new List<List<Tuple<int, int>>>();
                int list_num = 0;

                // Loop through each wall to search for a T and put possible doors into the list
                for (int i = 0; i < 4; i++) {
                    bool skip_room = false;
                    if (!walls[i]) {
                        switch (i) {
                            case 0:
                                // Loop to find a T
                                door_list.Add(new List<Tuple<int, int>>());
                                for (int j = 0; j < r.Width; j++) {
                                    if (gen[rooms[room_num].Y - 2,rooms[room_num].X + j] != 1) {
                                        if (skip_room) {
                                            list_num++;
                                            door_list.Add(new List<Tuple<int, int>>());
                                        }
                                        door_list[list_num].Add(new Tuple<int, int>(rooms[room_num].X + j, rooms[room_num].Y - 1));
                                        skip_room = false;
                                    } else {
                                        skip_room = true;
                                    }
                                }
                                list_num++;
                                break;
                            case 1:
                                door_list.Add(new List<Tuple<int, int>>());
                                for (int j = 0; j < r.Width; j++) {
                                    if (gen[rooms[room_num].Y + r.Height + 1,rooms[room_num].X + j] != 1) {
                                        if (skip_room) {
                                            list_num++;
                                            door_list.Add(new List<Tuple<int, int>>());
                                        }
                                        door_list[list_num].Add(new Tuple<int, int>(rooms[room_num].X + j, rooms[room_num].Y + r.Height));
                                        skip_room = false;
                                    } else {
                                        skip_room = true;
                                    }
                                }
                                list_num++;
                                break;
                            case 2:
                                door_list.Add(new List<Tuple<int, int>>());
                                for (int j = 0; j < r.Height; j++) {
                                    if (gen[rooms[room_num].Y + j,rooms[room_num].X - 2] != 1) {
                                        if (skip_room) {
                                            list_num++;
                                            door_list.Add(new List<Tuple<int, int>>());
                                        }
                                        door_list[list_num].Add(new Tuple<int, int>(rooms[room_num].X - 1, rooms[room_num].Y + j));
                                        skip_room = false;
                                    } else {
                                        skip_room = true;
                                    }
                                }
                                list_num++;
                                break;
                            case 3:
                                door_list.Add(new List<Tuple<int, int>>());
                                for (int j = 0; j < r.Height; j++) {
                                    if (gen[rooms[room_num].Y + j,rooms[room_num].X + r.Width + 1] != 1) {
                                        if (skip_room) {
                                            list_num++;
                                            door_list.Add(new List<Tuple<int, int>>());
                                        }
                                        door_list[list_num].Add(new Tuple<int, int>(rooms[room_num].X + r.Width, rooms[room_num].Y + j));
                                        skip_room = false;
                                    } else {
                                        skip_room = true;
                                    }
                                }
                                list_num++;
                                break;
                        }
                            
                    }
                }

                // Generate number of doors
                int num_doors = rand.Next(1, 3);

                // Remove empty lists
                for (int i = 0; i < door_list.Count; i++) {
                    if (door_list[i].Count == 0) {
                        door_list.RemoveAt(i);
                    }
                }

                //Loop through and insert walls
                int used_wall = -1;
                for (int i = 0; i < num_doors; i++) {
                    int door_coord1 = 0;
                    // 
                    do {
                        door_coord1 = rand.Next(0, door_list.Count);
                    } while (door_coord1 == used_wall);
                    used_wall = door_coord1;
                    int door_coord2 = rand.Next(0, door_list[door_coord1].Count);
                    gen[door_list[door_coord1][door_coord2].Item2, door_list[door_coord1][door_coord2].Item1] = 2;
                }

                // Output to check
                // foreach (var list in door_list) {
                //     foreach (var tuple in list) {
                //         Console.Write(tuple.Item1 + " " + tuple.Item2 + ", ");
                //     }
                //     Console.Write("  here \n" );
                // }
                // Console.Write(rooms[room_num].X + " " + rooms[room_num].Y +"  ------  " + r.Width + " " + r.Height + " " + num_doors + "\n");
                
            }
            room_num++;

        }
        // initialize rooms
        foreach(Room r in rooms) {
            switch(r.Type) {
                case Room_Type.Start: // initialize Start Room
                    break;
                case Room_Type.Exit: // initialize Exit Room
                    break;
                case Room_Type.Maze: // initialize Maze Room
                    break;
            }
        }

        // print standard map
        Console.WriteLine("reg map:");
        for(int y=0; y<gen.GetLength(0); y++) {
            string temp = "";
            for(int x=0; x<gen.GetLength(1); x++) {
                if(gen[y,x] == 0) temp += ' ';
                else temp += gen[y,x];
                temp += " ";
            }
            Console.WriteLine(temp);
        }
      
        // convert standard map to a triangle map
        Tyle[,] map = new Tyle[height,width];
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                map[y,x] = new Tyle();
                map[y,x].Left = gen[y,x];
                map[y,x].Right = gen[y,x];
            }
        }
        Console.WriteLine();

        //print triangle map
        // for(int di=1; di<width+height; di++) {
        //     string temp = "";
        //     for(int i = 0; i < (di<=height?height-di:di-height); i++)
        //         temp += ("  ");
        //     for(int ro=0; ro<(di<(height>width?width:height)?di:( di-(width>height?height:width) < Math.Abs(width-height) ? (width>height?height:width) : Math.Abs(di-width-height) )); ro++) {
        //         temp += map[(di<height?di:height)-ro-1,ro+(di>height?di-height:0)].Left==0?"  ":map[(di<height?di:height)-ro-1,ro+(di>height?di-height:0)].Left+" ";
        //         temp += map[(di<height?di:height)-ro-1,ro+(di>height?di-height:0)].Right==0?"  ":map[(di<height?di:height)-ro-1,ro+(di>height?di-height:0)].Right+" ";
        //     }
        //     Console.WriteLine(temp);
        // }
        Console.WriteLine("\nrooms: "+rooms.Count);
        
    }
}