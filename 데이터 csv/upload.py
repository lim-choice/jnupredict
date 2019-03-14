# -*- coding: utf-8 -*-
"""
Created on Tue Sep 11 17:30:45 2018

@author: lab401
"""

import sys
import pymysql
import csv
from pprint import pprint

#db host information
forrds_host = "baseball-prediction.cncwyzazxn7z.ap-northeast-2.rds.amazonaws.com"
forname = "root"
forpassword = "lab4012017"
fordb_name = "baseball"

#db에 접속
fordb = pymysql.connect(forrds_host, user=forname, passwd=forpassword, db=fordb_name, connect_timeout=5, charset = "utf8")
curs = fordb.cursor()

def csvtodb(filename):

    """
    Open a csv file and load it into a sql table.
    Assumptions:
     - the first line in the file is a header
    """

    f = csv.reader(open(filename))
    
    for line in f:
        for i in range(0, len(line)):
            if line[i] == '-':
                line[i] = '0'
        
        #타자
        #query_str = "INSERT INTO batter_stat VALUES ('" + line[0] + "', '" + line[1] + "', '" + line[2] + "', '" + line[3] + "', '" + line[4] + "', '" + line[5] + "', '" + line[6] + "', '" + line[7] + "', '" + line[8] + "', '" + line[9] + "', '" + line[10] + "', '" + line[11] + "', '" + line[12] + "', '" + line[13] + "', '" + line[14] + "', '" + line[15] + "', '" + line[16] + "', '" + line[17] + "', '" + line[18] + "', '" + line[19] + "')"
        
        #투수
        query_str = "INSERT INTO pitcher_stat VALUES ('" + line[0] + "', '" + line[1] + "', '" + line[2] + "', '" + line[3] + "', '" + line[4] + "', '" + line[5] + "', '" + line[6] + "', '" + line[7] + "', '" + line[8] + "', '" + line[9] + "', '" + line[10] + "', '" + line[11] + "', '" + line[12] + "', '" + line[13] + "', '" + line[14] + "', '" + line[15] + "', '" + line[16] + "', '" + line[17] + "', '" + line[18] + "', '" + line[19] + "', '" + line[20] + "')"
        print(query_str)
        curs.execute(query_str)
        fordb.commit()
    
    return


#csvtodb('D:\졸업작품\데이터 csv\player_batter.csv')
csvtodb('D:\졸업작품\데이터 csv\player_pitcher.csv')

#db commit
curs.close()