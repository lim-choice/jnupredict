#라이브러리, 패키지 가져옴
import numpy as np
import matplotlib.pyplot as plt
import pandas as pd
from sklearn.preprocessing import MinMaxScaler


# 1. 데이터와 학습모델 불러옴
from keras.models import load_model

#데이터
dataset_train = pd.read_csv('data/train_data.csv') # 학습데이터
dataset_test = pd.read_csv('data/test_data.csv') # 테스트데이터

dataset_train.dropna(inplace=True) #NaN을 모두 제거: 데이터 안에 NaN이 있는 경우


#필요한 데이터 선택 추출
#real_obj_value_SO2 = dataset_test.iloc[:, 4:5].values
#real_obj_value_PM10 = dataset_test.iloc[:, 5:6].values
real_obj_value_PM25 = dataset_test.iloc[:, 6:7].values
#real_obj_value_O3 = dataset_test.iloc[:, 7:8].values
#real_obj_value_NO2 = dataset_test.iloc[:, 8:9].values
#real_obj_value_CO = dataset_test.iloc[:, 9:10].values

real_obj_value = real_obj_value_PM25

#파라미터
TIME_STEP = 7


#필요한 데이터 선택 추출
#training_set_SO2 = dataset_train.iloc[:, 4:5].values
#training_set_PM10 = dataset_train.iloc[:, 5:6].values
training_set_PM25 = dataset_train.iloc[:, 9:10].values
#training_set_O3 = dataset_train.iloc[:, 6:7].values
#training_set_NO2 = dataset_train.iloc[:, 7:8].values
#training_set_CO = dataset_train.iloc[:, 8:9].values

training_set = training_set_PM25


sc = MinMaxScaler(feature_range = (0, 1))
training_set_scaled = sc.fit_transform(training_set)


#원하는 자료 선택
#regressor = load_model('weight/SO2_jisan.h5')
regressor = load_model('weight/PM10_jisan.h5')
#regressor = load_model('weight/PM25_jisan.h5')
#regressor = load_model('weight/NO2_jisan.h5')
#regressor = load_model('weight/O3_jisan.h5')
#regressor = load_model('weight/CO_jisan.h5')


# 테스트 데이터 셋을 이용하여 예측값 구하기 
dataset_combined = pd.concat((dataset_train['PM2.5'], dataset_test['PM2.5']), axis = 0)

# input_data : 테스트 데이터 수 + TIME_STEP
input_obj = dataset_combined[len(dataset_combined) - len(dataset_test) - TIME_STEP:].values 
input_obj = input_obj.reshape(-1,1) #((테스트 데이터 수 + TIME_STEP),1)
input_obj = sc.transform(input_obj) #scale

#input_PM = X_test_scaled
X_test = []
for i in range(TIME_STEP, TIME_STEP+len(dataset_test)):X_test.append(input_obj[i-TIME_STEP:i, 0])
X_test = np.array(X_test)
X_test = np.reshape(X_test, (X_test.shape[0], X_test.shape[1], 1))
predicted_obj = regressor.predict(X_test)
predicted_obj = sc.inverse_transform(predicted_obj)

'''
   모델 성능 평가하기 위한 방법 (RMSE)
   import math
   from sklearn.metrics import mean_squared_error
   rmse = math.sqrt(mean_squared_error(real_PM_value, predicted_PM25))
'''

# 예측된 결과와 실제 값을 그래프로 표시
plt.plot(real_obj_value, color = 'red', label = 'Actural PM2.5')
plt.plot(predicted_obj, color = 'blue', label = 'Predicted PM2.5')
plt.title('PM2.5 Prediction System')
plt.xlabel('Time')
plt.ylabel('PM2.5')
plt.legend()
plt.show()




