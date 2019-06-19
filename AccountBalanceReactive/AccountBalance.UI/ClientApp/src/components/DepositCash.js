import React, { Component } from 'react';

export class DepositCash extends Component {

    constructor(props){
        super(props);

        this.state = {
            accountId: '',
            fund: 0,
            message: ''
        };
    }    

    componentWillReceiveProps(){
        if(this.state.accountId !== this.props.account[0].accountId)
        {
            this.setState({accountId: this.props.account[0].accountId,
                fund: 0,
                message: ''}); 
        }
    }

    depositCash = (event) => {
        event.preventDefault();
        
        if(this.state.fund === '' || this.state.fund === 0)
        {
            this.setState({message: <p style={{color: 'red'}}>Please enter amount</p>});
            return;
        }

        this.postData('api/Home/DepositCash', {accountId: this.props.account[0].accountId, fund: this.state.fund})
            .then(data => this.setState({message: data})) //JSON-string from `response.json()` call
            .catch(error => console.error(error));

        this.setState({fund: 0});
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

    fundOnChange = (event) => {
        this.setState({message: ''});

        const re = /^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$/;
        if (event.target.value === '' || event.target.value === 0 || re.test(event.target.value)) {
            this.setState({fund: event.target.value});        
        }
    }

    render(){
        return(
            <div>
                <table className='table table-accounts-action'>
                    <tbody>
                        <tr>
                            <td style={{textAlign: 'right', width: '315px'}}>Deposit fund: </td>
                            <td><input type="text" value={this.state.fund} 
                                onChange={event => this.fundOnChange(event)} required></input>
                            </td>                            
                        </tr>
                        <tr>
                            <td></td>
                            <td><button onClick={this.depositCash}>Deposit Cash</button></td>
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