import streamlit as st
from llama_cpp import Llama , StoppingCriteria , StoppingCriteriaList

llm = Llama(model_path="C:/Users/kerem/Desktop/llmhotel/Model/llama-2-7b-guanaco-qlora.Q4_K_M.gguf" , n_ctx=2048)
st.title("Assistant")


# Initialize the chat messages history
if "messages" not in st.session_state.keys():
    st.session_state.messages = [
        {"role": "assistant", "content": "How can I help?"}
    ]
    
    

# Prompt for user input and save
if prompt := st.chat_input():
    st.session_state.messages.append({"role": "user", "content": prompt})
    
    

# display the prior chat messages
for message in st.session_state.messages:
    with st.chat_message(message["role"]):
        st.write(message["content"])    
        
        
        
# If last message is not from assistant, we need to generate a new response
if st.session_state.messages[-1]["role"] != "assistant":
    # Call LLM
    with st.chat_message("assistant"):
        with st.spinner("Thinking..."):
            
            user_messages = [message["content"] for message in st.session_state.messages if message["role"] == "user"]
            
            user_input = "\n".join(user_messages).encode("utf-8")
            
           
            tokens = llm.tokenize(user_input)
           

            
            # response_tokens = llm.generate(tokens, top_k=40, top_p=0.95, temp=1.0, repeat_penalty=1.1,stopping_criteria=None)
        
            max_tokens = 512  
            
            
            response_tokens = []
            for token in llm.generate(tokens, top_k=40, top_p=0.95, temp=1.0, repeat_penalty=1.1):
                response_tokens.append(token)
                if len(response_tokens) >= max_tokens:
                    break
            
           
            response_text = llm.detokenize(response_tokens).decode("utf-8")
            
            st.write(response_text)

    message = {"role": "assistant", "content": response_text}
    st.session_state.messages.append(message)