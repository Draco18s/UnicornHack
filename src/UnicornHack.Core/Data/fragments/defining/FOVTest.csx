new DefiningMapFragment
{
    Name = "FOVTest",
    GenerationWeight = new BranchWeight { W = new ConstantWeight { }, Name = "dungeon", MinDepth = 1, MaxDepth = 1 },
    CreatureGenerator = new CreatureGenerator { ExpectedInitialCount = 0},
    ItemGenerator = new ItemGenerator { ExpectedInitialCount = 0 },
    NoRandomDoorways = true,
    Map = @"
#####                                       #####################
#...###################                     #...................#
###...................#######################.......#...#...#...#
  ###################...........<.........................>.....#
                    ############A############.......#...#...#...#
                          #...#...#...#     #...................#
                          #..#.....#..#     ######.###.##########
                          #.#...#...#.#     ,,,,, ,   ,
                        ####.........####   ,   , ,  , ,
                       ##.A.....#.....A..,,,,,,,,,, ,   ,
                      ##.###.........##..##     , ,,     ,
                     ##.###.#...#...#.##..##    ,,,       ,
                    ##.## #..#.....#..###..##     ,        ,
                   ##.##  #...#.#.#...# ##..##    ,         ,
                  ##.##   #....#.#....#  ##..##   ,          ,
###################.############A###########..####.###########,##
#...........#.......#...........................................#
#...........####.####........#.#.#.#.#.#.#......................#
#...............................................................#
#...........#.#.#.#.#........#.#.#.#.#.#.#......................#
#.....###..............#####....................................#
#...#.....#..................#.#.#.#.#.#.#......................#
#...#.....#...........#.....#...................................#
#...#.....#...........#.....##.#.#.#.#.#.#......................#
#...#.....#...........#.....#...................................#
#...#.....#..................#.#.#.#.#.#.#......................#
#.....###..............#####....................................#
#............................#.#.#.#.#.#.#......................#
#...............................................................#
#...............................................................#
#...............................................................#
#...............................................................#
#...............................................................#
#...............................................................#
#...............................................................#
#...............................................................#
#...............................................................#
#...............................................................#
#...............................................................#
#################################################################
"
}
