import React, { Component } from 'react';

export class DailyWireTransferLimit extends React.Component {

    constructor(props){
        super(props);

        this.state = {
            accountId: '',
            dailyWireTransferLimit: '000',
            message: ''
        };
    }    

    componentWillReceiveProps(){
        if(this.state.dailyWireTransferLimit === '000' || this.state.accountId !== this.props.account[0].accountId)
        {
            this.setState({accountId: this.props.account[0].accountId,
                dailyWireTransferLimit: this.props.account[0].dailyWireTransferLimit,
                message: ''}); 
        }
    }

    setDailyWireTransferLimit = (event) => {
        event.preventDefault();

        if(this.state.dailyWireTransferLimit === '' || this.state.dailyWireTransferLimit === 0)
        {
            this.setState({message: <p style={{color: 'red'}}>Please enter daily wire transfer limit</p>});
            return;
        }

        this.postData('api/Home/SetDailyWireTransferLimit', {accountId: this.props.account[0].accountId, dailyWireTransferLimit: this.state.dailyWireTransferLimit})
            .then(data => this.setState({message: data})) //JSON-string from `response.json()` call
            .catch(error => console.error(error));
    }

    postData = (url = '', data = {}) => {
        // Default options are marked with *
        return fetch(url, {
                method: 'POST', // *GET, POST, PUT, DELETE, etc.
                mode: 'cors', // no-cors, cors, *same-origin
                cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
                credentials: 'same-origin', // include, *same-origin, omit                
                headers: {
                    'Content-Type': 'application/json',//'application/json',
                    // 'Content-Type': 'application/x-www-form-urlencoded',
                },
                redirect: 'follow', // manual, *follow, error
                referrer: 'no-referrer', // no-referrer, *client
                body: JSON.stringify(data), // body data type must match "Content-Type" header
            })
            .then(response => response.json()); //response.json() parses JSON response into native Javascript objects 
    }

    limitOnChange = (event) => {
        this.setState({message: ''});

        const re = /^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$/;
        if (event.target.value === '' || event.target.value === 0 || re.test(event.target.value)) {
            this.setState({dailyWireTransferLimit: event.target.value});        
        }
    }

    render(){
        return(
            <div>
                <table className='table table-striped'>
                    <tbody>
                        <tr>
                            <td>Daily wire transfer limit:</td>
                            <td><input type="text" value={this.state.dailyWireTransferLimit} 
                                onChange={event => this.limitOnChange(event)} required></input>
                            </td>                            
                        </tr>
                        <tr>
                            <td></td>
                            <td><button onClick={this.setDailyWireTransferLimit}>Set limit</button></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>{this.state.message}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }
}