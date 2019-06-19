import React, { Component } from 'react';

export class OverdraftLimit extends Component {

    constructor(props){
        super(props);

        this.state = {
            accountId: '',
            overdraftLimit: '000',
            message: ''
        };
    }
    

    componentWillReceiveProps(){
        if(this.state.overdraftLimit === '000' || this.state.accountId !== this.props.account[0].accountId)
        {
            this.setState({accountId: this.props.account[0].accountId,
                overdraftLimit: this.props.account[0].overdraftLimit,
                message: ''}); 
        }
    }

    setOverdraftLimit = (event) => {
        event.preventDefault();

        if(this.state.overdraftLimit === '' || this.state.overdraftLimit === 0)
        {
            this.setState({message: <p style={{color: 'red'}}>Please enter overdraft limit</p>});
            return;
        }

        this.postData('api/Home/SetOverdraftLimit', {accountId: this.props.account[0].accountId, overdraftLimit: this.state.overdraftLimit})
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
            this.setState({overdraftLimit: event.target.value});        
        }
    }

    render(){
        return(
            <div>
                <table className='table table-accounts-action'>
                    <tbody>
                        <tr>
                            <td style={{textAlign: 'right', width: '315px'}}>Overdraft limit:</td>
                            <td><input type="text" value={this.state.overdraftLimit} 
                                onChange={event => this.limitOnChange(event)} required></input>
                            </td>                            
                        </tr>
                        <tr>
                            <td></td>
                            <td><button onClick={this.setOverdraftLimit}>Set Overdraft Limit</button></td>
                        </tr>
                        <tr style={{height:'50px'}}>
                            <td></td>
                            <td>{this.state.message}</td>
                        </tr>
                        <tr style={{height: '109px'}}>
                            <td></td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }
}