import React, {useState} from 'react';
import Button from './Button';
import Display from './Display';

export default function App () {
  const [count, setCount] = useState(10);
  const increamentCount = (increamentValue) => setCount(count + increamentValue);
  return (
    <div>
      This is a sample stateful and
      server-side rendered React application.
      <br /><br />
      Here is a button that will track how many times you click it:
      <br /><br />
      <Button onClickFunction={increamentCount} increament={1}/>
      <Button onClickFunction={increamentCount} increament={5}/>
      <Button onClickFunction={increamentCount} increament={10}/>
      <Button onClickFunction={increamentCount} increament={100}/>
      <br /><br />
      <Display message={count}/>
    </div>
  );  
}