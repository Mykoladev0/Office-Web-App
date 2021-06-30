import React from 'react';

import numeral from 'numeral';
import { CustomInput } from '../../../Vendor/mr-pro/components';

export interface ABKCInputProps {
  readonly: boolean;
  value: string;
  changeFn?: (val: string) => void;
}

type Props = Partial<ABKCInputProps>;

const ABKCInput = (props: Props): JSX.Element => {
  const parsedNumeral = numeral(props.value);
  let inputVal: any = parsedNumeral.value();
  inputVal = inputVal || '';
  const { changeFn, readonly } = props;
  return (
    <CustomInput
      labelText={'ABKC #'}
      id="abkcNo"
      formControlProps={{
        fullWidth: true,
        disabled: readonly,
      }}
      inputProps={{
        onChange: event => {
          if (!event.target.value) {
            changeFn('');
            return;
          }
          if ((event.target.value as string).length < 7) {
            let rtnVal: string = '';
            if (numeral.validate(event.target.value, '')) {
              rtnVal = numeral(event.target.value).format('000000,0');
            }
            if (changeFn && typeof changeFn === 'function') {
              changeFn(rtnVal);
            }
          }
        },
        type: 'number', //sets the input field type (html5 input types!)
        value: inputVal,
      }}
    />
  );
};

export { ABKCInput };
