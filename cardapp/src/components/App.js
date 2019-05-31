import React from 'react';
import '../styles/App.css';


	const testData = [
			{name: "Dan Abramov", avatar_url: "https://avatars0.githubusercontent.com/u/810438?v=4", company: "@facebook"},
      {name: "Sophie Alpert", avatar_url: "https://avatars2.githubusercontent.com/u/6820?v=4", company: "Humu"},
  		{name: "Sebastian Markbåge", avatar_url: "https://avatars2.githubusercontent.com/u/63648?v=4", company: "Facebook"},
	];

const CardList = (props) => (
  <div>
    <Card {...testData[0]}/>
    <Card {...testData[1]}/>
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

class App extends React.Component{
  render(){
    return (
      <div>
        <div style={{textAlign: 'center', fontSize: '1.5rem', marginBottom: '1rem'}}>{this.props.title}</div>
        <CardList />
      </div>
    );
  }
}

export default App;