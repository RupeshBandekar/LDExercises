import React from 'react';
import '../styles/App.css';


	const testData = [
			{name: "Dan Abramov", avatar_url: "https://avatars0.githubusercontent.com/u/810438?v=4", company: "@facebook"},
      {name: "Sophie Alpert", avatar_url: "https://avatars2.githubusercontent.com/u/6820?v=4", company: "Humu"},
  		{name: "Sebastian Markbåge", avatar_url: "https://avatars2.githubusercontent.com/u/63648?v=4", company: "Facebook"},
	];

const CardList = (props) => (
  <div>
    {props.profiles.map(profile => <Card {...profile}/>)}
  </div>
);

class Card extends React.Component{
  render(){
    const profile = this.props;
    return(      
      <div style={{margin: '1rem'}}>
        <img style={{width: '75px'}} src={profile.avatar_url} />
        <div style={{display: 'inline-block', marginLeft:'12px'}}>
          <div style={{fontSize: '1.25rem', fontWeight: 'bold'}}>{profile.name}</div>
          <div style={{boxSizing: 'border-box'}}>{profile.company}</div>
        </div>
      </div>
    );
  }
}

class Form extends React.Component{
  state = {userName: ''};
  handleSubmit = (event) =>{
    event.preventDefault();
    console.log(this.state.userName);
  };
    
  render(){    
    return(
      <form onSubmit={this.handleSubmit}>
        <input 
        type="text" 
        value={this.state.userName}
        onChange={event => this.setState({userName: event.target.value})}
        placeholder="Username"
        required>          
        </input>
        <button>Add card</button>
      </form>
    );
  }
}

class App extends React.Component{
  /* constructor(props){
    super(props);
    this.state = {
      profiles: testData,
    };
  }; */

  state ={
    profiles: testData,
  };
  
  render(){
    return (
      <div>
        <div style={{textAlign: 'center', fontSize: '1.5rem', marginBottom: '1rem'}}>{this.props.title}</div>
        <Form />
        <CardList profiles={this.state.profiles}/>
      </div>
    );
  }
}

export default App;