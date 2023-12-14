import os

from modelscope.pipelines import pipeline
from modelscope.utils.constant import Tasks
from fastapi import FastAPI, UploadFile
import uuid
import soundfile
import io
AudioToText_pipeline = pipeline(
        task=Tasks.auto_speech_recognition,
        model='damo/speech_paraformer-large-vad-punc_asr_nat-zh-cn-16k-common-vocab8404-pytorch',
        model_revision="v1.2.4")

# print(AudioToText_pipeline(audio_in = "test.wav"))

app = FastAPI()

@app.post("/audio2text")
async def audio2text(File: UploadFile):
    file_id = uuid.uuid4()
    print("[INFO]收到audio2text请求, 分配id:"+str(file_id)+", 开始处理")
    print("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++")
    file_content = await File.read()
    filename = str(file_id)+".wav"
    with open(filename, "wb") as f:
        f.write(file_content)
    f.close()

    result = AudioToText_pipeline(audio_in = filename)
    del result['text_postprocessed']
    del result['time_stamp']
    for item in result['sentences']:
        ts_list = item['ts_list']
        ts_first = ts_list[0][0]
        item['start'] = ts_first
        del item['text_seg']
        del item['ts_list']
    # print(result)
    try:
        os.remove(filename)
        print("[INFO]删除临时音频文件："+filename)
    except FileNotFoundError:
        print(f"File '{filename}' not found.")
    except Exception as e:
        print(f"An error occurred: {e}")
    print("[INFO]audio2text请求处理完毕，请求id："+str(file_id))
    return result