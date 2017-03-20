new EncompassingMapFragment
{
    Name = "d1",
    GenerationWeight = new BranchWeight { W = new ConstantWeight { }, Name = "dungeon", MinDepth = 1, MaxDepth = 1 },
    NoRandomDoorways = true,
    LevelHeight = 40,
    LevelWidth = 80,
    Map = @"
                     ###########                 #######
                     #..........,,,,,,,,,,,,,,,,,....%..,,
 ########           ##..=......#,,   #####,      #.....# ,
 #......#            #..........,,,, #.B.#,      #.....# ,,,
 #......#            #.........#,  , #>...,      ##.####   ,    ###########
 #......#  ,         ########.##,  ,,....#,,,,,,,,,,,,,,,,,,,,,,..........#
 #......., ,,,,,,,,,,,,,,,,,,,,,,   ,#####,        ,,,       ,  #......b..#
 #...b..#,                   ,      ,,,,  ,          ,       ,,,#.........#
 #.$....#,                   ,         ,  ,          ,,,       ,.......(..#
 ######.#,,,                 ,         ,,,,            ,        #.........#
       ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,#    ###########
           ,,, ##############.#           , ###########.#####
             , #...............,,,,,,,,   , #..............#
             ,,....b..........#           , #$#.....b...<..#
               #........).....#           ,,...............#
               #...%..........#             ################
               #####.##########
                   #,
                    ,
                    ,
                    ,
                    ,
                    ,
                    ,
                    ,
               #####.#######
               #...........#
               #........$..#
               #...........#
               #########.###
                        ,
                        ,
                        ,
                        ,
                        .#####
                        #....#
                        #....#
                        #..B.#
                        #....#
                        ######
"
}
