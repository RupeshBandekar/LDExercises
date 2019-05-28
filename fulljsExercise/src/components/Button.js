import React from 'react';

export default function Button(props){
  const handleClick = () => props.onClickFunction(props.increament);
    return (
      <button onClick={handleClick}>
          +{props.increament}
      </button>
    );
  }