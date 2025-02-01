# /qna/create_embedding_data.py

import pandas as pd
from tqdm import tqdm
tqdm.pandas()

import torch
from sentence_transformers import SentenceTransformer

train_file = "C:/Users/user/pythonProject/csvTrans/qua/micobot.xlsx"
model = SentenceTransformer('snunlp/KR-SBERT-V40K-klueNLI-augSTS')

df = pd.read_excel(train_file)
df['embedding_vector'] = df['Q'].progress_map(lambda x : model.encode(x))

embedding_data = torch.tensor(df['embedding_vector'].tolist())
torch.save(embedding_data, 'micobot_embedding_data.pt')
