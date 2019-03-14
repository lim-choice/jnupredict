# -*- coding: utf-8 -*-

# load moduls
import pandas as pd
import numpy as np


#%%


# 실제 데이터 불러오기
dataset = pd.read_csv("data/last_data.csv")

# 데이터 배열만들기
x_set = dataset.iloc[:, 1:125].values


# Taking care of miising data : replace NaN to col average
col_mean = np.nanmean(x_set, axis=0)
inds = np.where(np.isnan(x_set))
x_set[inds] = np.take(col_mean, inds[1])


predict_x_set = x_set


#%%

# 스케일링
from sklearn.preprocessing import MinMaxScaler as sc

# data scaling(0-1)
scaled = sc()
predict_x_set = scaled.fit_transform(predict_x_set)


#%%

from keras.models import load_model

# load value_weight
model = load_model('data/weight_spes.h5')


#%%

# 모델 사용
predict_y_set = model.predict_classes(predict_x_set)

# 예측값 출력
#for i in range (5):
#    print ('예측값 : ' + str(predict_y_set))

print ('예측값 : ' + str(predict_y_set))

print("예측 확률:\n{}".format(model.predict_proba(predict_x_set)))
