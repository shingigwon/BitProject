import torch
import numpy as np
import pandas as pd
from sentence_transformers import SentenceTransformer, util


# model = SentenceTransformer.load('D:/mcmproject/Micobot/model')
model = SentenceTransformer.load('snunlp/KR-SBERT-V40K-klueNLI-augSTS')
embedding_data = torch.load('D:/mcmproject/Micobot/micobot_embedding_data.pt')
df = pd.read_excel('D:/mcmproject/Micobot/micobot.xlsx')

def return_answer(text):
    sentence = text
    sentence_encode = model.encode(sentence)
    sentence_tensor = torch.tensor(sentence_encode)
    cos_sim = util.cos_sim(sentence_tensor, embedding_data)

    best_sim_idx = int(np.argmax(cos_sim))
    answer = df['A'][best_sim_idx]
    code = df['code'][best_sim_idx]
    return answer, code

