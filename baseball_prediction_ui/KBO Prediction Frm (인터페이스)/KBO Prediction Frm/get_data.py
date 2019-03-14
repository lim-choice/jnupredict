# -*- coding: utf-8 -*-
"""
Created on Mon Oct  1 18:23:34 2018

@author: lab401
"""

# load moduls
import requests as rq
from bs4 import BeautifulSoup
import pandas as pd
import pymysql
import json, csv
from pprint import pprint
from sklearn.preprocessing import MinMaxScaler as sc
import numpy as np
import pathlib
import os
from datetime import datetime

#crawling url

#db host information
forrds_host = "baseball-prediction.cncwyzazxn7z.ap-northeast-2.rds.amazonaws.com"
forname = "root"
forpassword = "lab4012017"
fordb_name = "baseball"

#db에 접속
fordb = pymysql.connect(forrds_host, user=forname, passwd=forpassword,\
                        db=fordb_name, connect_timeout=5, charset = "utf8")

curs = fordb.cursor()

top_url = 'https://sports.news.naver.com/kbaseball/index.nhn'

def crawler():
    res = rq.get(top_url)
    soup = BeautifulSoup(res.text, 'lxml')
                                            
    infoTable = soup.find("div",{"class":"hmb_list"})
        
    team_name = []
    team_player = []
    temp_list = []
    flag = 0
    
    #1차 분류    
    for a in infoTable.find_all("span"):
        temp_list.append(str(a).split('<span')[1])
        
    #2차 분류
    for a in temp_list:
        #print(a)
        if flag == 0 or flag == 2:
            #print(str(a).split('class="name">')[1].split('</span>'))
            #print(str(a))
            team_name.append(str(a).split('class="name">')[1].split('</span>')[0])
            pass
        elif flag == 1 or flag == 3:
            #print(str(a).split('<span>')[0].split('</span>')[0].split('>')[1])
            team_player.append(str(a).split('<span>')[0].split('</span>')[0].split('>')[1])
            pass
        else:
            pass
       
        flag += 1
        
        if flag >= 5:
            flag = 0
            
    team_name.reverse()
    team_player.reverse()
                
    return team_name, team_player

def match(home_name, home_player, away_name, away_player):

    
    line_to_csv = []
    
    line_to_csv.append(str(datetime.today().strftime('%Y%m%d'))) #날짜
    
    #홈팀 타자
    query_str = "SELECT name FROM batter_log WHERE date = '20180915' and my_team = '" + home_name + "'"
    curs.execute(query_str)
    rows = curs.fetchall()
    
    bat_player_list = rows
            
    for j in range(0, 9):
        for yi in range(2018, 2019):
            query_str = "SELECT * FROM batter_stat WHERE year = '" + str(yi) +\
            "' and name = '" + bat_player_list[j][0] + "'"
            
            curs.execute(query_str)
            rows = curs.fetchall()
            
            bat_player_in_team = rows
            
            if len(bat_player_in_team) != 0:
                break
            
        #만약 데이터가 없을 경우에 
        if len(bat_player_in_team) == 0:
            line_to_csv.append('0') #원래는 NaN
            line_to_csv.append('0') #원래는 NaN
            line_to_csv.append('0') #원래는 NaN
            line_to_csv.append('0') #원래는 NaN
            line_to_csv.append('0') #원래는 NaN
            line_to_csv.append('0') #원래는 NaN
        else:
            #필요한 부분만 정보 뺴오기 (타자)
            line_to_csv.append(rows[0][15])  #출루율
            line_to_csv.append(rows[0][16])  #장타율
            line_to_csv.append(rows[0][14])  #타율
            
            if(rows[0][5] == 0):
                line_to_csv.append(0)
                line_to_csv.append(0)
                line_to_csv.append(0)
            else:
                line_to_csv.append(round(rows[0][11]/rows[0][5], 3))  #사구(볼넷)/타수
                line_to_csv.append(round(rows[0][10]/rows[0][5], 3))  #타점/타수
                line_to_csv.append(round(rows[0][12]/rows[0][5], 3))  #삼진/타수        
    
    #홈팀 투수        
    for yj in range(2018, 2019):
        query_str = "SELECT * FROM pitcher_stat WHERE year = '" + str(yj) +\
        "' and name = '" + home_player + "'"
        
        curs.execute(query_str)
        rows = curs.fetchall()
        
        pit_player_in_team = rows
        
        #만약 투수 데이터가 없다면
        if len(pit_player_in_team) != 0:
            break
        
    #필요한 부분만 정보 뺴오기 (타자)
    if len(pit_player_in_team) == 0:
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
    else:
        line_to_csv.append(rows[0][15])  #피안타율
        
        if(rows[0][8] == 0):
            line_to_csv.append('0') #원래는 NaN
        else:
            line_to_csv.append(round(rows[0][3]/rows[0][8], 3))   #선발승률
        
        if(rows[0][9] == 0):
            line_to_csv.append('0') #원래는 NaN
        else:
            line_to_csv.append(round(rows[0][10]/rows[0][9], 3))  #평균소화이닝
            
        line_to_csv.append(rows[0][16])  #평균자책점
        line_to_csv.append(rows[0][12])  #볼넷/이닝
        line_to_csv.append(rows[0][11])  #삼진/이닝
        line_to_csv.append(rows[0][13])  #피홈런/이닝
        line_to_csv.append(rows[0][15])  #잔류처리율
        
        
    #어웨이팀 타자
    query_str = "SELECT name FROM batter_log WHERE date = '20180915' and my_team = '" + away_name + "'"
    curs.execute(query_str)
    rows = curs.fetchall()
    
    bat_player_list = rows
            
    for j in range(0, 9):
        for yi in range(2018, 2019):
            query_str = "SELECT * FROM batter_stat WHERE year = '" + str(yi) +\
            "' and name = '" + bat_player_list[j][0] + "'"
            
            curs.execute(query_str)
            rows = curs.fetchall()
            
            bat_player_in_team = rows
            
            if len(bat_player_in_team) != 0:
                break
            
        #만약 데이터가 없을 경우에 
        if len(bat_player_in_team) == 0:
            line_to_csv.append('0') #원래는 NaN
            line_to_csv.append('0') #원래는 NaN
            line_to_csv.append('0') #원래는 NaN
            line_to_csv.append('0') #원래는 NaN
            line_to_csv.append('0') #원래는 NaN
            line_to_csv.append('0') #원래는 NaN
        else:
            #필요한 부분만 정보 뺴오기 (타자)
            line_to_csv.append(rows[0][15])  #출루율
            line_to_csv.append(rows[0][16])  #장타율
            line_to_csv.append(rows[0][14])  #타율
            
            if(rows[0][5] == 0):
                line_to_csv.append(0)
                line_to_csv.append(0)
                line_to_csv.append(0)
            else:
                line_to_csv.append(round(rows[0][11]/rows[0][5], 3))  #사구(볼넷)/타수
                line_to_csv.append(round(rows[0][10]/rows[0][5], 3))  #타점/타수
                line_to_csv.append(round(rows[0][12]/rows[0][5], 3))  #삼진/타수        
    
    #어웨이팀 투수        
    for yj in range(2018, 2019):
        query_str = "SELECT * FROM pitcher_stat WHERE year = '" + str(yj) +\
        "' and name = '" + away_player + "'"
        
        curs.execute(query_str)
        rows = curs.fetchall()
        
        pit_player_in_team = rows
        
        #만약 투수 데이터가 없다면
        if len(pit_player_in_team) != 0:
            break
        
    #필요한 부분만 정보 뺴오기 (타자)
    if len(pit_player_in_team) == 0:
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
        line_to_csv.append('0') #원래는 NaN
    else:
        line_to_csv.append(rows[0][15])  #피안타율
        
        if(rows[0][8] == 0):
            line_to_csv.append('0') #원래는 NaN
        else:
            line_to_csv.append(round(rows[0][3]/rows[0][8], 3))   #선발승률
        
        if(rows[0][9] == 0):
            line_to_csv.append('0') #원래는 NaN
        else:
            line_to_csv.append(round(rows[0][10]/rows[0][9], 3))  #평균소화이닝
            
        line_to_csv.append(rows[0][16])  #평균자책점
        line_to_csv.append(rows[0][12])  #볼넷/이닝
        line_to_csv.append(rows[0][11])  #삼진/이닝
        line_to_csv.append(rows[0][13])  #피홈런/이닝
        line_to_csv.append(rows[0][15])  #잔류처리율
        
    
    #print(line_to_csv)
    convert_line_to_csv(line_to_csv)
    
    pass


"""
    DB에서 csv 데이터를 가져옴
"""

def clear_csv():
    file = 'data/last_data.csv'
    
    if os.path.isfile(file):
        path = pathlib.Path(file)
        path.unlink()
        
    return
        
def clear_text():
    file = 'result.txt'
    
    if os.path.isfile(file):
        path = pathlib.Path(file)
        path.unlink()
        
    return

def convert_line_to_csv(line):
    
    print(line)
    
    myCsvRow = ''
    for i in range(0, len(line)):
        myCsvRow = myCsvRow + str(line[i])
        
        if i != len(line)-1:
            myCsvRow = myCsvRow + ','
        
    
    #csv_register
    print(myCsvRow)
    fd = open('data/last_data.csv','a')
    fd.write(myCsvRow + '\n')
    fd.close()
        
    return


def prediction(file, team_name, team_player):
    
    # 실제 데이터 불러오기
    dataset = pd.read_csv("data/last_data.csv")
    
    # 데이터 배열만들기
    x_set = dataset.iloc[:, 1:125].values
    
    # Taking care of miising data : replace NaN to col average
    col_mean = np.nanmean(x_set, axis=0)
    inds = np.where(np.isnan(x_set))
    
    x_set[inds] = np.take(col_mean, inds[1])    
    predict_x_set = x_set
    
    from keras.models import load_model
    # load value_weight
    model = load_model('data/weight_spes.h5')
    
    # 모델 사용
    predict_y_set = model.predict_classes(predict_x_set)
    print('예측값 : ' + str(predict_y_set))
    
    testlist = model.predict_proba(predict_x_set)
    print("예측 확률:\n{}".format(testlist))
    
    if testlist[0][0] > testlist[0][2]:
        winner_team = team_name[0]
    elif testlist[0][0] < testlist[0][2]:
        winner_team = team_name[1]
    else:
        winner_team = '무승부'
    
    #예측 후 해당 파일 텍스트로 쓰기
    clear_text()
    fw = open('result.txt', 'w')
    fw.write(str(datetime.today().strftime('%Y%m%d')) + ',' + str(team_name[0]) + ',' + str(team_player[0]) + ',' + str(team_name[1]) + ',' + str(team_player[1]) + ',' + str(winner_team) + ',' + str(round(testlist[0][0], 3)) + ',' + str(round(testlist[0][1], 3)) + ',' + str(round(testlist[0][2], 3)))
    fw.close()
    
    print(str(datetime.today().strftime('%Y%m%d')), team_name, team_player)
    

def upload_predict_full_db(_list):
    #게임 정보 가져오고 db에 send 후 commit()
    print(len(_list))
    
    query_str = "select count(*) from game_log"
    curs.execute(query_str)
    rows = curs.fetchall()
        
    data = int(rows[0][0])

    for number in range(0, data):
        #db에 접속해서 값 긁어오기
        query_str = "select home_team, away_team from game_log where id = '" + str(number) + "'"
        curs.execute(query_str)
        team = curs.fetchall()
        
        home_team = team[0][0]
        away_team = team[0][1]
        
        #예측 부분 (csv 읽어서 리스트 화 한거 분석)
        predict_win_per = _list[number][0]
        predict_draw_per = _list[number][1]
        predict_lose_per = _list[number][2]
                
        if predict_win_per > predict_lose_per:
            predict_winteam = home_team
            predict_loseteam = away_team
        elif predict_win_per < predict_lose_per:
            predict_winteam = away_team
            predict_loseteam = home_team
        else:
            predict_winteam = ''
            predict_loseteam = ''
            
        predict_win_per = round(float(predict_win_per), 3)
        predict_draw_per = round(float(predict_draw_per), 3)
        predict_lose_per = round(float(predict_lose_per), 3)
        
        #sql로 값 업데이트 시키기
        query_str2 = "UPDATE game_log SET predict_win_team = '" + str(predict_winteam) +\
        "', predict_lose_team = '" + str(predict_loseteam) + "', predict_win_per = '" +\
        str(predict_win_per) + "', predict_draw_per = '" + str(predict_draw_per) +\
        "', predict_lose_per = '" + str(predict_lose_per) + "' where id = '" +\
        str(number) + "'"
        
        print(query_str2)
        
        curs.execute(query_str2)
        fordb.commit()
        

team_name = []
team_player = []

if __name__ == "__main__":
    
    #데이터 당일 크롤링 부분
    team_name, team_player = crawler()
    
    clear_csv()
    convert_line_to_csv(['','H1OBP','H1SLG','H1BA','H1BB','H1RBI','H1SO','H2OBP','H2SLG','H2BA','H2BB','H2RBI',\
                         'H2SO','H3OBP','H3SLG','H3BA','H3BB','H3RBI','H3SO','H4OBP','H4SLG','H4BA','H4BB','H4RBI',\
                         'H4SO','H5OBP','H5SLG','H5BA','H5BB','H5RBI','H5SO','H6OBP','H6SLG','H6BA','H6BB','H6RBI',\
                         'H6SO','H7OBP','H7SLG','H7BA','H7BB','H7RBI','H7SO','H8OBP','H8SLG','H8BA','H8BB','H8RBI',\
                         'H8SO','H9OBP','H9SLG','H9BA','H9BB','H9RBI','H9SO','HPOBA','HPSW','HPNOI','HPERA','HPBB',\
                         'HPK','HPOHA','HPBF','A1OBP','A1SLG','A1BA','A1BB','A1RBI','A1SO','A2OBP','A2SLG','A2BA',\
                         'A2BB','A2RBI','A2SO','A3OBP','A3SLG','A3BA','A3BB','A3RBI','A3SO','A4OBP','A4SLG','A4BA',\
                         'A4BB','A4RBI','A4SO','A5OBP','A5SLG','A5BA','A5BB','A5RBI','A5SO','A6OBP','A6SLG','A6BA',\
                         'A6BB','A6RBI','A6SO','A7OBP','A7SLG','A7BA','A7BB','A7RBI','A7SO','A8OBP','A8SLG','A8BA',\
                         'A8BB','A8RBI','A8SO','A9OBP','A9SLG','A9BA','A9BB','A9RBI','A9SO','APOBA','APSW','APNOI',\
                         'APERA','APBB','APK','APOHA','APBF','WIN', 'DRAW', 'LOSE'])
    
    for i in range(0, len(team_name)):
        if (i % 2) == 0:
            match(team_name[i], team_player[i], team_name[i+1], team_player[i+1])
    
    prediction('test.csv', team_name, team_player)
    
    
    
    
    
    
    
    