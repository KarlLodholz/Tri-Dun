using System;
using System.Collections.Generic;

public class Tyle {
    public int Left,Right;
}

public class Room{
    public int Width, Height;
    public int X,Y;
}

public class Program {

    public const int MAP_MAX = 25;
    public const int MAP_MIN = 17;

    public const int ROOM_MAX = 5;
    public const int ROOM_MIN = 3;

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
                rooms.Add(new Room() {Width = Right, Height = down, X = addresses.Peek()%width, Y = addresses.Peek()/width});
                addresses.Dequeue();
            }
            else { // the retancle needs to be divided
                // where wall will created after being added to pos
                int div = rand.Next(ROOM_MIN, ROOM_MAX);
                pos = addresses.Peek();
                if(Right > down) { //cut vertically
                    // if rectangle being created is big enough add to queue
                    if( width-(pos%width+div+1) > ROOM_MIN ) addresses.Enqueue(pos+div+1);
                    while(gen[pos/width, pos%width+div] != 1) {
                        gen[pos/width, pos%width+div] = 1;
                        pos+=width;
                    }
                } else { //cut horizontally
                    // if rectangle being created is big enough add to queue
                    if( height-(pos/width+div+1) > ROOM_MIN ) addresses.Enqueue(pos+(div+1)*width);
                    while(gen[pos/width+div, pos%width] != 1) {
                        gen[pos/width+div, pos%width] = 1;
                        pos++;
                    }
                }
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
        for(int di=1; di<width+height; di++) {
            string temp = "";
            for(int i = 0; i < (di<=height?height-di:di-height); i++)
                temp += ("  ");
            for(int ro=0; ro<(di<(height>width?width:height)?di:( di-(width>height?height:width) < Math.Abs(width-height) ? (width>height?height:width) : Math.Abs(di-width-height) )); ro++) {
                temp += map[(di<height?di:height)-ro-1,ro+(di>height?di-height:0)].Left==0?"  ":map[(di<height?di:height)-ro-1,ro+(di>height?di-height:0)].Left+" ";
                temp += map[(di<height?di:height)-ro-1,ro+(di>height?di-height:0)].Right==0?"  ":map[(di<height?di:height)-ro-1,ro+(di>height?di-height:0)].Right+" ";
            }
            Console.WriteLine(temp);
        }
        Console.WriteLine("\nrooms: "+rooms.Count);
        // for(int i = 0; i < width+height-1; i++) {
        //     string temp = "";
        //     for(int k = 0; k < Math.Abs(height - 1 - i); k++) {
        //         temp += "  ";
        //     }
        //     for(int j = 0; j < (width>height?height:width); j++) {
        //         temp += map[(i<height?i:(height-j)),j].Left + " " + map[(i<height?i:(height-j)),j].Right + " ";
        //     }
        //     Console.WriteLine(temp);
        // }

        // // convert standard map to a triangle map
        // int[][] map = new int[height*2][];
        // for(int y=0; y<height; y++) {
        //     map[2*y] = new int[width-(y%2==1 ? 1 : 0)];
        //     map[2*y+1] = new int[width-(y%2==1 ? 0 : 1)];
        //     for(int x=0; x<width; x++) {
        //         if(x!=width-1 || y%2==0)
        //             map[2*y][x] = (y%2==0 ? gen[y,x] : ((gen[y,x]==1 || gen[y,x+1]==1) ? 1 : gen[y,x]));
        //         if(x!=width-1 || y%2!=0) 
        //             map[2*y+1][x] = (y%2!=0 ? gen[y,x] : ((gen[y,x]==1 || gen[y,x+1]==1) ? 1 : gen[y,x]));
        //     }
        // }
        
        // // print triangle map
        // Console.WriteLine("\ntriangle converted: ");
        // for(int y=0; y<map.Length; y++) {
        //     string temp = y%4==1||y%4==2 ? "   ":"";
        //     for(int x=0; x<map[y].Length; x++) {
        //         temp += map[y][x];
        //         temp += "     ";
        //     }
        //     Console.WriteLine(temp);
        // }
        
    }
}