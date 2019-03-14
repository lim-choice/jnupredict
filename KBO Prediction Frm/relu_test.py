# -*- coding: utf-8 -*-
"""
Created on Wed Oct  3 15:34:20 2018

@author: Lab401
"""
# load moduls
import numpy as np
import pandas as pd

from keras.models import Sequential
from keras.layers import Dense
from keras.layers import Dropout

from sklearn.preprocessing import MinMaxScaler as sc

np.random.seed(7)

#%%

# load data
# train_data : 2375개, test_data : 288개
dataset = pd.read_csv("data/document_for_learning.csv")


# setting data_set
training_x_set = dataset.iloc[:2375, 1:125].values
training_y_set = dataset.iloc[:2375, 125:128].values
test_x_set = dataset.iloc[2375:, 1:125].values
test_y_set = dataset.iloc[2375:, 125:128].values


# Taking care of miising data : replace NaN to col average
col_mean = np.nanmean(training_x_set, axis=0)
inds = np.where(np.isnan(training_x_set))
training_x_set[inds] = np.take(col_mean, inds[1])

col_mean2 = np.nanmean(test_x_set, axis=0)
inds = np.where(np.isnan(test_x_set))
test_x_set[inds] = np.take(col_mean2, inds[1])


# data scaling(0-1)
scaled = sc()
training_x_set = scaled.fit_transform(training_x_set)

#%%

# making learning model
model = Sequential()
# 입력값 == ((6*9)+(8*1)) * 2
model.add(Dense(100, input_dim = 124, activation = 'relu'))
model.add(Dense(100, activation = 'relu'))
model.add(Dense(100, activation = 'relu'))
model.add(Dropout(0.1))
model.add(Dense(100, activation = 'relu'))
model.add(Dropout(0.2))
model.add(Dense(80, activation = 'relu'))
model.add(Dropout(0.2))
model.add(Dense(80, activation = 'relu'))
model.add(Dropout(0.2))
model.add(Dense(80, activation = 'relu'))
model.add(Dropout(0.2))
model.add(Dense(34, activation = 'relu'))
model.add(Dropout(0.1))
model.add(Dense(20, activation = 'relu'))
model.add(Dense(3, activation = 'relu'))
model.add(Dense(3, activation = 'softmax'))


# optimizer setting
#opt = 'Adam'
#opt = 'sgd'
#opt = 'AdaGrad'
opt = 'RMSprop'
#opt = 'Adadelta'
#opt = 'Nadam'

model.compile(loss = 'binary_crossentropy', optimizer = opt, metrics = ['accuracy'])

epochs = 50
his = model.fit(training_x_set, training_y_set, epochs=epochs, batch_size=124)

# print Learning assessment
scores = model.evaluate(test_x_set, test_y_set)
print("%s: %.2f%%" %(model.metrics_names[1], scores[1]*100))

print("예측 확률:\n{0:.4f}".format(model.predict_proba(training_x_set)))

#%%


# module for graph
import matplotlib.pyplot as plt

fig, loss_ax = plt.subplots()
acc_ax = loss_ax.twinx()
loss_ax.plot(his.history['loss'], 'y', label='train loss')
acc_ax.plot(his.history['acc'], 'b', label='train acc')
loss_ax.set_xlabel('epoch')
loss_ax.set_ylabel('loss')
acc_ax.set_ylabel('accuray')

plt.rcParams["figure.figsize"] = (15,5)
plt.title(opt)

plt.show()

#save model of train
#model.save('data/catego/opt-' + str(opt) + '_epochs-' + str(epochs) + '_weight.h5')


model.save('data/weight_spes.h5')
